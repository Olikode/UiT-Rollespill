using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Text dialogText;
    [Header("Selectors")]
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [Header("Action Text")]
    [SerializeField] List<Text> actionText;
    [Header("Moves")]
    [SerializeField] GameObject moveDetails;
    [SerializeField] List<Text> moveText;
    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    public void SetDialog(string dialog)
    {
        // sets dialog instantly
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialg)
    {
        // types dialog char by char
        dialogText.text = "";
        foreach (var letter in dialg.ToCharArray())
        {
            dialogText.text += letter;
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return))
            {
                yield return new WaitForSeconds(1f / (letterPerSecond*10));
            }
            else
            {
                yield return new WaitForSeconds(1f / (letterPerSecond));
            }
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enable)
    {
        dialogText.enabled = enable;
    }

    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    public void EnableMoveSelector(bool enable)
    {
        moveSelector.SetActive(enable);
        moveDetails.SetActive(enable);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        // highlight currently selected action
        for (int i = 0; i < actionText.Count; ++i)
        {
            if (i == selectedAction)
                actionText[i].color = GlobalSettings.i.HighlightedColor;
            else
                actionText[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        // highlights currently selected move
        for (int i = 0; i < moveText.Count; ++i)
        {
            if (i == selectedMove)
                moveText[i].color = GlobalSettings.i.HighlightedColor;
            else
                moveText[i].color = Color.black;
        }

        // show info of currently selected move
        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else    
            ppText.color = Color.black;
        
    }

    public void SetMoveNames(List<Move> moves)
    {
        // set name of move on attack button
        for (int i = 0; i < moveText.Count; i++)
        {
            if (i < moves.Count)
                moveText[i].text = moves[i].Base.Name;
            else
                moveText[i].text = "-";
        }
    }
}
