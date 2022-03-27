using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text lvlText;
    [SerializeField] HPBar hpBar;


    public void SetData(Unit1 unit){
        //nameText.text = unit.Base.Name;
        lvlText.text = "Lvl " + unit.Level;
        hpBar.SetHp((float)unit.HP / unit.MaxHP);
    }

}
