using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    MoveToForget,
    BattleOver,
    Bag,
}

public enum BattleAction { Move, UseItem, Run}

public class BattleSystem : MonoBehaviour
{
    [Header("Units")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;

    [Header("")]
    [SerializeField] GameObject challengerImage;
    [SerializeField] BattleDialogBox dialogBox;

    [Header("Moves")]
    [SerializeField] LearnMoveUI learnMoveUI;
    [SerializeField] GameObject LearnMoveInfo;

    [Header("Inventory")]
    [SerializeField] InventoryUI inventoryUI;


    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    UnitList player;
    UnitList enemy;

    Challenger challenger;

    bool isExamBattle = false;

    MoveBase moveToLearn;

    public void StartBattle(UnitList player, UnitList enemy)
    {
        // stat battle with wild enemy
        this.player = player;
        this.enemy = enemy;

        isExamBattle = false;

        StartCoroutine(SetupBattle());
    }

    public void StartExamBattle(UnitList player, UnitList enemy, Challenger enemyChallenger)
    {
        // start battle with challenger
        this.player = player;
        this.enemy = enemy;
        this.challenger = enemyChallenger;
        
        isExamBattle = true;

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if(!isExamBattle){
            // wild enemy
            playerUnit.Setup(player.GetPlayerUnit());
            enemyUnit.Setup(enemy.GetHealthyUnit());

            dialogBox.SetMoveNames(playerUnit.Unit.Moves);

            yield return dialogBox.TypeDialog($"En vill {enemyUnit.Unit.Base.Name} dukket opp");
        }
        else{
            // challenger
            enemyUnit.gameObject.SetActive(false);
            playerUnit.SetupNoAnimation(player.GetPlayerUnit());

            challengerImage.GetComponent<Image>().sprite = challenger.Sprite;
            challengerImage.gameObject.SetActive(true);
            challengerImage.GetComponent<BattleUnit>().ChallengerResetPos();

            yield return dialogBox.TypeDialog($"{challenger.Prefix} {challenger.Name} utfordrer deg");

            // challenger sends out units
            challengerImage.GetComponent<BattleUnit>().PlayLeaveAnimation();
            enemyUnit.gameObject.SetActive(true);

            var enemyUnits = enemy.GetHealthyUnit();
            enemyUnit.Setup(enemyUnits);
            

            yield return dialogBox.TypeDialog($"{challenger.Name} sendte ut en {enemyUnits.Base.Name}");
            challengerImage.gameObject.SetActive(false);
            dialogBox.SetMoveNames(playerUnit.Unit.Moves);
        }

        ActionSelection();
    }

    void BattleOver(bool isWon)
    {
        state = BattleState.BattleOver;
        playerUnit.Unit.OnBattleOver();
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(isWon);
    }

    void ActionSelection()
    {
        // player select action
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Hva vil du gjøre?"));

        dialogBox.EnableActionSelector(true);
    }

    void MoveSelection()
    {
        // player select move
        state = BattleState.MoveSelection;

        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);

        dialogBox.EnableMoveSelector(true);
    }

    void OpenBag()
    {
        // shows player inventory
        inventoryUI.gameObject.SetActive(true);
        state = BattleState.Bag;
    }

    IEnumerator RunTurns(BattleAction playerAction){
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Unit.CurrentMove = playerUnit.Unit.Moves[currentMove];
            enemyUnit.Unit.CurrentMove = enemyUnit.Unit.GetRandomMove();

            int playerMovePriority = playerUnit.Unit.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Unit.CurrentMove.Base.Priority;

            // check who attacks first
            // if move priority is higher, that move always goes first
            bool playerFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerFirst = false;
            else if (enemyMovePriority == playerMovePriority){
                playerFirst = playerUnit.Unit.Speed >= enemyUnit.Unit.Speed;
            }
    
            var firstUnit = (playerFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerFirst) ? enemyUnit : playerUnit;

            // store secondUnit in variable in case it is killed before its turn and enemy switch out new unit
            var SU = secondUnit.Unit;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Unit.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            // check if secound unit is still alive
            if(SU.HP > 0){
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Unit.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                yield return dialogBox.TypeDialog($"Du brukte {inventoryUI.itemName}");
                yield return dialogBox.TypeDialog($"{inventoryUI.itemMessage}");
            }
            else {
                // TODO: add more actions
            }

            // Enemy turn
            var enemyMove = enemyUnit.Unit.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if(state == BattleState.BattleOver) yield break;

        }

        if(state != BattleState.BattleOver)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit attacker, BattleUnit defender, Move move)
    {
        // runs turn of player and enemy
        bool canRunMove = attacker.Unit.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(attacker.Unit);
            yield break;
        }
        yield return ShowStatusChanges(attacker.Unit);

        move.PP--;
        yield return dialogBox.TypeDialog($"{attacker.Unit.Base.Name} bruker {move.Base.Name}");

        if (CheckIfMoveHits(move, attacker.Unit, defender.Unit)){

            attacker.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            defender.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, attacker.Unit, defender.Unit, move.Base.Target);
            }
            else if (move.Base.Category == MoveCategory.Normal)
            {
                var damageDetails = defender.Unit.TakeDamage(move, attacker.Unit);
                yield return defender.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);

                if(defender.Unit.Status?.Id != null){
                    if(defender.Unit.Status.Id.Equals(ConditionID.Søvn)){
                        var random = UnityEngine.Random.Range(0,2);
                        if(random == 1){
                            defender.Unit.StatusTime -= 1;
                            yield return dialogBox.TypeDialog($"{defender.Unit.Base.Name} vrir seg i søvne");
                        }   
                    }
                }
            }

            if (move.Base.SecondaryEffects != null && defender.Unit.HP > 0){
                var random = UnityEngine.Random.Range(1, 101);
                if(random <= move.Base.SecondaryEffects.Chance)
                    yield return RunMoveEffects(move.Base.SecondaryEffects, attacker.Unit, defender.Unit, move.Base.Target);
            }

            if (defender.Unit.HP <= 0)
            {
                yield return HandleUnitFainted(defender);
            }

        }
        else{
            yield return dialogBox.TypeDialog($"{attacker.Unit.Base.Name} sitt angrep bommet");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Unit attacker, Unit defender, MoveTarget target)
    {
        // Stat boosting
        if (effects.Boosts != null)
        {
            if (target == MoveTarget.Self)
            {
                attacker.ApplyBoosts(effects.Boosts);
            }
            else
            {
                defender.ApplyBoosts(effects.Boosts);
            }
        }

        // Status condition
        if (effects.Status != ConditionID.Null)
        {
            defender.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(attacker);
        yield return ShowStatusChanges(defender);
    }

    IEnumerator RunAfterTurn(BattleUnit attacker){

        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Some status effects will change HP of the unit after the turn
            attacker.Unit.OnAfterTurn();
            yield return ShowStatusChanges(attacker.Unit);
            yield return attacker.Hud.WaitForHPUpdate();

            if (attacker.Unit.HP <= 0)
            {
                yield return HandleUnitFainted(attacker);
                yield return new WaitUntil(() => state == BattleState.RunningTurn);
            }
    }

    IEnumerator ChooseMoveToForget(MoveBase newMove)
    {
        // when player level up and can choose new move
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Velg et angrep å glemme");
        learnMoveUI.gameObject.SetActive(true);
        learnMoveUI.SetMoveData(playerUnit.Unit.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    bool CheckIfMoveHits(Move move, Unit attacker, Unit defender){
        //calculates if attack hits or not based on move stats
        //formula based on pokemon games

        if (move.Base.AlwaysHit == true)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = attacker.StatBoosts[Stat.Treffsikkerhet];
        int evasion = defender.StatBoosts[Stat.Unnvikelse];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if(accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if(evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.StatusChanges.Count > 0)
        {
            var message = unit.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandleUnitFainted(BattleUnit faintedUnit)
    {
        // Show player unit who lost 
        yield return dialogBox.TypeDialog($"{faintedUnit.Unit.Base.Name} har tapt");
        faintedUnit.PlayDieAnimation();
        yield return new WaitForSeconds(2f);

        // Gain Exp if fainted unit is not the player
        if(!faintedUnit.IsPlayer)
        {
            // Exp gain
            int expYield = faintedUnit.Unit.Base.ExpYield;
            int levelBonus = faintedUnit.Unit.Level;
            float examBonus = (isExamBattle)? 1.5f : 0.5f;

            int expGain = Mathf.FloorToInt((expYield * levelBonus * examBonus)/10);
            Debug.Log("exp " + expGain);
            playerUnit.Unit.Exp += expGain;
            yield return dialogBox.TypeDialog($"Hjernekraften din økte med {expGain}");
            yield return playerUnit.Hud.SetExpSmooth();


            // Level up
            while (playerUnit.Unit.CheckForLevelUp()){
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"Du er nå level {playerUnit.Unit.Level}");

                // Learn new move
                var newMove = playerUnit.Unit.GetCurrentLevelMove();
                if (newMove != null)
                {
                    // Check if player can learn new move
                    if (playerUnit.Unit.Moves.Count < UnitBase.MaxNumOfMoves)
                    {
                        playerUnit.Unit.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"Du har lært {newMove.Base.Name}");
                        dialogBox.SetMoveNames(playerUnit.Unit.Moves);
                    }
                    else
                    {
                        yield return dialogBox.TypeDialog($"Du prøver å lære {newMove.Base.Name}");
                        yield return dialogBox.TypeDialog($"Men du kan ikke lære mer. Vil du glemme noe for å lære {newMove.Base.Name}");
                        yield return ChooseMoveToForget(newMove.Base);

                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                    }
                }
                yield return playerUnit.Hud.SetExpSmooth(true);
            }
        }
        CheckForBattleOver(faintedUnit);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            BattleOver(false);
        }
        else
        {
            if (!isExamBattle){
                // TODO add loot drops

                playerUnit.Unit.CureStatus();
                BattleOver(true);
            }
            else{
                // check if enemy have more units
                var nextUnit = enemy.GetHealthyUnit();
                if(nextUnit != null)
                    StartCoroutine(sendOutNextUnit(nextUnit));
                else
                    BattleOver(true);
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("Et kritisk treff");
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action onItemUsed = () =>
            {
                state = BattleState.Busy;
                inventoryUI.gameObject.SetActive(false);
                StartCoroutine(RunTurns(BattleAction.UseItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                learnMoveUI.gameObject.SetActive(false);
                if(moveIndex == UnitBase.MaxNumOfMoves)
                {
                    StartCoroutine(dialogBox.TypeDialog($"Du glemmer ingenting"));
                }
                else
                {
                    // forgets selected move, learns new move
                    var selectedMove = playerUnit.Unit.Moves[moveIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog($"Du glemte {selectedMove.Name} og lærte {moveToLearn.Name}"));

                    playerUnit.Unit.Moves[moveIndex] = new Move(moveToLearn);
                    dialogBox.SetMoveNames(playerUnit.Unit.Moves);
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            learnMoveUI.HandleMoveSelection(onMoveSelected);
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (currentAction == 0)
            {
                MoveSelection();
            }
            else if (currentAction == 1) 
            { 
                OpenBag();
            }
        }
    }

    void HandleMoveSelection()
    {
        // TODO add move like struggle, when player is out of moves with PP
        // From Bulbapedia: It damages an enemy, but it also damages you. 
        // (If there's no move available, your move will be Struggle.)
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentMove < playerUnit.Unit.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentMove < playerUnit.Unit.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Unit.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            var move = playerUnit.Unit.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);

            dialogBox.EnableDialogText(true);

            StartCoroutine(RunTurns(BattleAction.Move));
        }
    }

    IEnumerator sendOutNextUnit(Unit nextUnit){
        state = BattleState.Busy;
        enemyUnit.Setup(nextUnit);
        yield return dialogBox.TypeDialog($"{challenger.Name} sendte ut {nextUnit.Base.Name}");

        state = BattleState.RunningTurn;
    }
}
