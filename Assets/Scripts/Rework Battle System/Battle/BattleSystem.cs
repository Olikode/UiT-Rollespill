using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    BattleUnit playerUnit;

    [SerializeField]
    BattleHud playerHud;

    [SerializeField]
    BattleUnit enemyUnit;

    [SerializeField]
    BattleHud enemyHud;

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
        this.enemy =  enemy;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(player.GetHealthyUnit());
        enemyUnit.Setup(enemy.GetHealthyUnit());
        playerHud.SetData(playerUnit.Unit);
        enemyHud.SetData(enemyUnit.Unit);

        dialogBox.SetMoveNames(playerUnit.Unit.Moves);

        yield return dialogBox.TypeDialog($"En vill {enemyUnit.Unit.Base.Name} dukket opp");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Hva vil du gjøre?"));

        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;

        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);

        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Unit.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"Du angriper med {move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.Unit.TakeDamage(move, playerUnit.Unit);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if(damageDetails.Fainted){
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} er beseiret");
            enemyUnit.PlayDieAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else{
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove(){
        state = BattleState.EnemyMove;

        var move = enemyUnit.Unit.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} angriper med {move.Base.Name}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();
        var damageDetails = playerUnit.Unit.TakeDamage(move, enemyUnit.Unit);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if(damageDetails.Fainted){
            yield return dialogBox.TypeDialog($"Du tapte");
            playerUnit.PlayDieAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else{
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails){
        if(damageDetails.Critical > 1f){
            yield return dialogBox.TypeDialog("Et kritisk treff");
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
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
                PlayerMove();
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

            StartCoroutine(PerformPlayerMove());
        }
    }
}
