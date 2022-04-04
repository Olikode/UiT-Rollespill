using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    BattleOver
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    BattleUnit playerUnit;

    [SerializeField]
    BattleUnit enemyUnit;

    [SerializeField]
    BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    UnitList player;
    UnitList enemy;

    public void StartBattle(UnitList player, UnitList enemy)
    {
        this.player = player;
        this.enemy = enemy;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(player.GetHealthyUnit());
        enemyUnit.Setup(enemy.GetHealthyUnit());

        dialogBox.SetMoveNames(playerUnit.Unit.Moves);

        yield return dialogBox.TypeDialog($"En vill {enemyUnit.Unit.Base.Name} dukket opp");

        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.Unit.Speed >= enemyUnit.Unit.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
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

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Unit.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Unit.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        if (state == BattleState.PerformMove)
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

        attacker.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        defender.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, attacker.Unit, defender.Unit);
        }
        else if (move.Base.Category == MoveCategory.Normal)
        {
            var damageDetails = defender.Unit.TakeDamage(move, attacker.Unit);
            yield return defender.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (defender.Unit.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{defender.Unit.Base.Name} har tapt");
            defender.PlayDieAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(defender);
        }

        // Some status effects will change HP of the unit after the turn
        defender.Unit.OnAfterTurn();
        yield return ShowStatusChanges(attacker.Unit);
        yield return attacker.Hud.UpdateHP();

        if (attacker.Unit.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{attacker.Unit.Base.Name} har tapt");
            attacker.PlayDieAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(defender);
        }
    }

    IEnumerator RunMoveEffects(Move move, Unit attacker, Unit defender)
    {
        var effects = move.Base.Effects;

        // Stat boosting
        if (move.Base.Effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
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

    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.StatusChanges.Count > 0)
        {
            var message = unit.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            BattleOver(false);
        }
        else
        {
            //check if oponent has more units
            BattleOver(true);
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
            else if (currentAction == 1) { }
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
            dialogBox.EnableMoveSelector(false);

            dialogBox.EnableDialogText(true);

            StartCoroutine(PlayerMove());
        }
    }
}
