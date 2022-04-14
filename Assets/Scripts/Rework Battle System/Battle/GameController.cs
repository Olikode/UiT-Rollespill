using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    [SerializeField]
    BattleSystem battleSystem;

    [SerializeField]
    Camera worldCamera;

    GameState state;

    private void Awake(){
        ConditionsDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        playerController.OnChallenged += StartExamBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    void StartBattle(UnitList enemy)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        playerController.inBattle = true;
        var player = playerController.GetComponent<UnitList>();

        battleSystem.StartBattle(player, enemy);
    }
    public void StartExamBattle(UnitList enemy, Challenger challenger)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        playerController.inBattle = true;
        var player = playerController.GetComponent<UnitList>();
        var enemyList = challenger.GetComponent<UnitList>();

        battleSystem.StartExamBattle(player, enemyList, challenger);
    }

    void EndBattle(bool isWon){
        state = GameState.FreeRoam;
        playerController.inBattle = false;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
