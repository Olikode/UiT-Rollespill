using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LearnMoveUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Text moveInfo;

    [SerializeField] Color highLigtedColor;
    int currentSelection = 0;


    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i=0; i<currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++ currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            -- currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, UnitBase.MaxNumOfMoves);

        UpdateMoveSelection(currentSelection);

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            onSelected?.Invoke(currentSelection);
    }

    public void UpdateMoveSelection(int selection)
    {
        for(int i = 0; i < UnitBase.MaxNumOfMoves + 1; i++)
        {
            if(i == selection)
                moveTexts[i].color = highLigtedColor;
            else   
                moveTexts[i].color = Color.black;
        }
    }
}
