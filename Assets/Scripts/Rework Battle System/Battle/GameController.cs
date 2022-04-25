using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
    Menu,
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    [SerializeField]
    BattleSystem battleSystem;

    [SerializeField]
    Camera worldCamera;
    
    MenuController menuController;

    GameState state;

    private void Awake(){

        menuController = GetComponent<MenuController>();

        ConditionsDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        playerController.OnChallenged += StartExamBattle;
        battleSystem.OnBattleOver += EndBattle;
        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menuController.onMenuSelected += OnMenuSelected;
    }

    void StartBattle(UnitList enemy)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        playerController.isPaused = true;
        var player = playerController.GetComponent<UnitList>();

        battleSystem.StartBattle(player, enemy);
    }
    public void StartExamBattle(UnitList enemy, Challenger challenger)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        playerController.isPaused = true;
        var player = playerController.GetComponent<UnitList>();
        var enemyList = challenger.GetComponent<UnitList>();

        battleSystem.StartExamBattle(player, enemyList, challenger);
    }

    void EndBattle(bool isWon){
        state = GameState.FreeRoam;
        playerController.isPaused = false;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
            playerController.isPaused = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }

        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Menu)
        {
            menuController.HandleUpdate();
            playerController.isPaused = true;
        }
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            // student-info
        }
        else if (selectedItem == 1)
        {
            // ryggsekk
        }
        else if (selectedItem == 2)
        {
            // save
            // TODO add load and save
        }
        else if (selectedItem == 3)
        {
            // load
            // TODO add load and save
        }

        state = GameState.FreeRoam;
    }
}
