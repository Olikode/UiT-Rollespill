using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    BattleOver
}

public enum BattleAction { Move, UseItem, Run}

public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    BattleUnit playerUnit;

    [SerializeField]
    BattleUnit enemyUnit;

    [SerializeField]
    GameObject challengerImage;

    [SerializeField]
    BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    UnitList player;
    UnitList enemy;

    Challenger challenger;

    bool isExamBattle = false;

    public void StartBattle(UnitList player, UnitList enemy)
    {
        this.player = player;
        this.enemy = enemy;

        isExamBattle = false;
        
        StartCoroutine(SetupBattle());
    }

    public void StartExamBattle(UnitList player, UnitList enemy, Challenger x)
    {
        this.player = player;
        this.enemy = enemy;
        this.challenger = x;
        
        isExamBattle = true;

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if(!isExamBattle){
            // wild enemy
            playerUnit.Setup(player.GetHealthyUnit());
            enemyUnit.Setup(enemy.GetHealthyUnit());

            dialogBox.SetMoveNames(playerUnit.Unit.Moves);

            yield return dialogBox.TypeDialog($"En vill {enemyUnit.Unit.Base.Name} dukket opp");
        }
        else{
            // challenger

            enemyUnit.gameObject.SetActive(false);

            challengerImage.GetComponent<Image>().sprite = challenger.Sprite;
            challengerImage.gameObject.SetActive(true);

            yield return dialogBox.TypeDialog($"{challenger.Prefix} {challenger.Name} utfordrer deg");

            // challenger sends out units
            challengerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);

            var enemyUnits = enemy.GetHealthyUnit();
            enemyUnit.Setup(enemyUnits);
            playerUnit.SetupNoAnimation(player.GetHealthyUnit());

            yield return dialogBox.TypeDialog($"{challenger.Name} sendte ut en {enemyUnits.Base.Name}");
            dialogBox.SetMoveNames(playerUnit.Unit.Moves);
        }

        ActionSelection();
    }

    void BattleOver(bool isWon)
    {
        state = BattleState.BattleOver;
        playerUnit.Unit.OnBattleOver();
        OnBattleOver(isWon);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Hva vil du gjøre?"));

        dialogBox.EnableActionSelector(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;

        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);

        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator RunTurns(BattleAction playerAction){
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move){
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
        else {
            // TODO: add more actions
            // Enemy switching unit
            // Using item
        }

        if(state != BattleState.BattleOver)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit attacker, BattleUnit defender, Move move)
    {
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
                yield return defender.Hud.UpdateHP();
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
            yield return attacker.Hud.UpdateHP();

            if (attacker.Unit.HP <= 0)
            {
                yield return HandleUnitFainted(attacker);
                yield return new WaitUntil(() => state == BattleState.RunningTurn);
            }
    }

    bool CheckIfMoveHits(Move move, Unit attacker, Unit defender){

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
        yield return dialogBox.TypeDialog($"{faintedUnit.Unit.Base.Name} har tapt");
        faintedUnit.PlayDieAnimation();
        yield return new WaitForSeconds(2f);


        if(!faintedUnit.IsPlayer)
        {
            // Exp gain
            int expYield = faintedUnit.Unit.Base.ExpYield;
            int levelBonus = faintedUnit.Unit.Level;
            float examBonus = (isExamBattle)? 1.5f : 0.5f;

            int expGain = Mathf.FloorToInt((expYield * levelBonus * examBonus)/10);
            Debug.Log("exp " + expGain);
            playerUnit.Unit.Exp += expGain;
            yield return dialogBox.TypeDialog($"Du har fått {expGain} studiepoeng");
            yield return playerUnit.Hud.SetExpSmooth();


            // Level up
            while (playerUnit.Unit.CheckForLevelUp()){
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"Du er nå level {playerUnit.Unit.Level}");

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
                playerUnit.Unit.CureStatus();
                BattleOver(true);
            }
            else{
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
                // ryggsekk (use item)
            }
        }
    }

    void HandleMoveSelection()
    {
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
