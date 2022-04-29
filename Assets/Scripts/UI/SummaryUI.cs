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

    [SerializeField] GameObject playerGO;
    Unit player;
    List<Move> moves;

    int selectedMove = 0;

    private void Start()
    {
        player = playerGO.GetComponent<UnitList>().GetHealthyUnit();
        moves = player.Moves;
        SetPlayerInfo(player);
    }
    
    public void SetPlayerInfo(Unit player)
    {
        nameText.text = $"{player.Base.Name}";
        typeText.text = $"{player.Base.Type}";
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

    public void HandleUpdate(Action onBack)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedMove;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedMove;

        selectedMove = Mathf.Clamp(selectedMove, 0, 3);

        UpdateMoveSlection();

        if(Input.GetKeyDown(KeyCode.Escape))
            onBack?.Invoke();
    }

    void UpdateMoveSlection()
    {
        for (int i = 0; i < moveInfoFields.Count; i++)
        {
            if (i == selectedMove)
                moveInfoFields[i].GetComponent<MoveInfoUI>().ShowSelected(GlobalSettings.i.HighlightedColor);
            else
                moveInfoFields[i].GetComponent<MoveInfoUI>().ShowSelected(Color.black);
        }
    }

}
