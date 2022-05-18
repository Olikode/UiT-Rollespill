using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SummaryUI : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] Image playerImage;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text typeText;
    [SerializeField] Text hpText;
    [Header("Moves")]
    [SerializeField] List<GameObject> moveInfoFields;
    [SerializeField] Text descriptionText;
    [SerializeField] Text accuracyText;
    [SerializeField] Text PowerText;
    [Header("Player gameobject")]
    [SerializeField] GameObject playerGO;

    Inventory inventory;
    Unit player;
    List<Move> moves;

    int selectedItem = -1;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
    }
    private void Start()
    {
        player = playerGO.GetComponent<UnitList>().GetPlayerUnit();
        moves = player.Moves;
        SetPlayerInfo();
        inventory.OnUpdated += SetPlayerInfo;
        player.OnHPChanged += SetPlayerInfo;
    }
    
    public void SetPlayerInfo()
    {
        playerImage.sprite = player.Base.Sprite;
        nameText.text = $"{player.Base.Name}";
        typeText.text = $"{player.Base.Type}-Student";
        levelText.text = $"Level {player.Level}";
        hpText.text = $"HP: {player.HP}/{player.MaxHP}";
        
        // set move info
        for (int i = 0; i < UnitBase.MaxNumOfMoves; i++)
        {
            string name;
            string pp;

            if(i < moves.Count)
            {
                name = moves[i].Base.Name;
                pp = $"PP: {moves[i].PP}/{moves[i].Base.PP}";
            }
            else
            {
                name = "";
                pp = "";
            }

            moveInfoFields[i].GetComponent<MoveInfoUI>().SetInfo(name, pp);
        }
    }

    public void HandleUpdate(Action onBack, Action onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
            if(selectedItem < 0)
                selectedItem = 0;
        }
        // switch between player and move list
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedItem = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(selectedItem == -1)
            selectedItem = 0;
        }

        selectedItem = Mathf.Clamp(selectedItem, -1, moves.Count-1);

        UpdateMoveSlection();

        if(Input.GetKeyDown(KeyCode.Escape))
            onBack?.Invoke();
        if(Input.GetKeyDown(KeyCode.Return))
            onSelected?.Invoke();
    }

    void UpdateMoveSlection()
    {
        for (int i = 0; i < moveInfoFields.Count; i++)
        {
            // If player is highlighted instead of moves
            if(selectedItem < 0)
            {
                nameText.color = GlobalSettings.i.HighlightedColor;
                descriptionText.text = "";
                accuracyText.text = $"Treffsikkerhet: -";
                PowerText.text = $"Kraft: -";
            }

            if (i == selectedItem)
            {
                nameText.color = Color.black;
                moveInfoFields[i].GetComponent<MoveInfoUI>().ShowSelected(GlobalSettings.i.HighlightedColor);
                descriptionText.text = moves[i].Base.Description;
                accuracyText.text = $"Treffsikkerhet: {moves[i].Base.Accuracy}";
                PowerText.text = $"Kraft: {moves[i].Base.Power}";
            }
            else
                moveInfoFields[i].GetComponent<MoveInfoUI>().ShowSelected(Color.black);
        }
    }
}
