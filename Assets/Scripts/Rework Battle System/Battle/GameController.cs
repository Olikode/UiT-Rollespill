using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum GameState
{
    FreeRoam,
    Battle,
    Menu,
    Bag,
    Summary,
    CharacterSelection,
    Dialog,
}

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    public static UnitList playerUnit;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;

    [SerializeField] Camera worldCamera;
    
    MenuController menuController;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] SummaryUI summaryUI;
    [SerializeField] CharacterSelectionUI characterSelectionUI;
    [SerializeField] DialougeManager dm;

    public static DialougeManager dialougeManager { get; set; }

    public static IInteractable Interactable { get; set; }

    GameState state;

    private void Awake(){

        menuController = GetComponent<MenuController>();

        ConditionsDB.Init();
        state = GameState.CharacterSelection;
    }

    private void Start()
    {
        playerUnit
 = playerController.gameObject.GetComponent<UnitList>();
        dialougeManager = dm;
        playerController.OnEncountered += StartBattle;
        playerController.OnChallenged += StartExamBattle;
        battleSystem.OnBattleOver += EndBattle;
        // back to freeroam state when not paused
        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menuController.onMenuSelected += OnMenuSelected;
        dialougeManager.onDialogClosed += () =>
        {
            if(state != GameState.Battle)
                state = GameState.FreeRoam;
        };
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

        battleSystem.StartExamBattle(player, enemy, challenger);
    }

    void EndBattle(bool isWon){
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        // checks current gamestate and runs pause function
        if(state == GameState.FreeRoam)
            playerController.isPaused = false;
        else   
            playerController.isPaused = true;


        if (state == GameState.FreeRoam)
        {
            inventoryUI.inBattle = false;
            playerController.HandleUpdate();
            

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }

            //If player presses M, the dialouge starts. Also checks if the dialouge is open, to stop the player from pressing M again, and ending up with double the text. 
            if (Input.GetKeyDown(KeyCode.E) && dialougeManager.IsOpen == false)
            {
                if (Interactable != null)
                {
                    Interactable.Interact(playerController);
                    state = GameState.Dialog;
                }
        }

        }
        else if (state == GameState.Battle)
        {
            dialougeManager.CloseDialougeBox();
            inventoryUI.inBattle = true;
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

            summaryUI.HandleUpdate(onBack, null);
        }
        else if (state == GameState.CharacterSelection)
        {            
            characterSelectionUI.gameObject.SetActive(true);
            characterSelectionUI.HandleUpdate();

            // when player has confirmed character
            if (characterSelectionUI.confirmedCharacter)
            {
                // creates a new UnitBase to be used as player
                var playerCharacter = ScriptableObject.CreateInstance<UnitBase>();

                var playerClass = characterSelectionUI.PlayerUnitBase;
                var playerImage = characterSelectionUI.PlayerImage;
                var playerName = characterSelectionUI.PlayerName;

                // creates the player character with correct stas
                playerCharacter.SetCharacterData(playerClass, playerName, playerImage);
                playerUnit.AddPlayerUnit(playerCharacter);
                playerController.SetPlayerSprite();

                // player can now controll character
                state = GameState.FreeRoam;
                characterSelectionUI.gameObject.SetActive(false);
            }
        }
        else if (state == GameState.Dialog)
        {
            dialougeManager.responseHandler.HandleUpdate();
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
    }
}
