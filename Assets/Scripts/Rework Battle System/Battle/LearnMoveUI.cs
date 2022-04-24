using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LearnMoveUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Text moveName;
    [SerializeField] Text moveDescription;
    [SerializeField] Text movePP;
    [SerializeField] Text movePower;
    [SerializeField] Text moveAccuracy;

    [SerializeField] Color highLigtedColor;
    int currentSelection = 0;

    List<MoveBase> allMoves = new List<MoveBase>();


    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i=0; i<currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].Name;
            allMoves.Add(currentMoves[i]);
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
        allMoves.Add(newMove);
        Debug.Log(allMoves);
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
            if(i == selection){
                moveTexts[i].color = highLigtedColor;
                moveName.text = allMoves[i].Name;
                moveDescription.text = allMoves[i].Description;
                movePP.text = $"PP: {allMoves[i].PP}";
                movePower.text = $"Kraft: {allMoves[i].Power}";
                moveAccuracy.text = $"Treffsikkerhet: {allMoves[i].Accuracy}";
            }
            else   
                moveTexts[i].color = Color.black;
        }
    }
}
