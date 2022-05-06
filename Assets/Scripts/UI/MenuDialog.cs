using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDialog : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;

    int letterPerSecond = 10;

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
}
