using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SummaryUI : MonoBehaviour
{
    [SerializeField] List<GameObject> moveInfoFields;

    [SerializeField] Image playerImage;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text typeText;
    [SerializeField] Text hpText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text accuracyText;
    [SerializeField] Text PowerText;
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
        player = playerGO.GetComponent<UnitList>().GetHealthyUnit();
        moves = player.Moves;
        SetPlayerInfo();
        inventory.OnUpdated += SetPlayerInfo;
        player.OnHPChanged += SetPlayerInfo;
    }
    
    public void SetPlayerInfo()
    {
        nameText.text = $"{player.Base.Name}";
        typeText.text = $"{player.Base.Type}-Student";
        levelText.text = $"Level {player.Level}";
        hpText.text = $"HP: {player.HP}/{player.MaxHP}";

        
        for (int i = 0; i < moves.Count; i++)
        {
            string name = moves[i].Base.Name;
            string pp = $"PP: {moves[i].PP}/{moves[i].Base.PP}";

            moveInfoFields[i].GetComponent<MoveInfoUI>().SetInfo(name, pp);

            /*if (moves[i].PP == 0)
                ppText.color = Color.red;
            else    
                ppText.color = Color.black;*/
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
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedItem = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(selectedItem == -1)
            selectedItem = 0;
        }

        selectedItem = Mathf.Clamp(selectedItem, -1, 3);

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
