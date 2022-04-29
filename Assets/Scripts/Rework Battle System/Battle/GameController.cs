using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState
{
    FreeRoam,
    Battle,
    Menu,
    Bag,
    Summary,
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
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] SummaryUI summaryUI;

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

        
        var player = playerController.GetComponent<UnitList>();

        battleSystem.StartBattle(player, enemy);
    }
    public void StartExamBattle(UnitList enemy, Challenger challenger)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        
        var player = playerController.GetComponent<UnitList>();
        var enemyList = challenger.GetComponent<UnitList>();

        battleSystem.StartExamBattle(player, enemyList, challenger);
    }

    void EndBattle(bool isWon){
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(state == GameState.FreeRoam)
            playerController.isPaused = false;
        else   
            playerController.isPaused = true;


        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
            

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
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.Menu;
                
            };
            inventoryUI.HandleUpdate(onBack);
        }
        else if (state == GameState.Summary)
        {
            Action onBack = () =>
            {
                summaryUI.gameObject.SetActive(false);
                state = GameState.Menu;
                
            };

            summaryUI.HandleUpdate(onBack);
        }
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            summaryUI.gameObject.SetActive(true);
            state = GameState.Summary;
        }
        else if (selectedItem == 1)
        {
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
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

        //state = GameState.FreeRoam;
    }
}
