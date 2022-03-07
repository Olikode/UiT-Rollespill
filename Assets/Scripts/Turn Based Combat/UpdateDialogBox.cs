using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateDialogBox : MonoBehaviour
{

    public Text dialogText;

    public void openAttackMenu(){
        dialogText.text = "Velg ditt angrep";
    }    

    public void openInventoryMenu(){
        dialogText.text = "Velg objekt fra ryggsekken din";
    }
}
