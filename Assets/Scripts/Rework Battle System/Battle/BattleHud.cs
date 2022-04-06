using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField]
    Text nameText;

    [SerializeField]
    Text lvlText;

    [SerializeField]
    Image conditionImage;

    [SerializeField]
    Sprite sleepIcon;

    [SerializeField]
    Sprite poisonIcon;

    [SerializeField]
    Sprite stunIcon;

    [SerializeField]
    Color iconColor;

    [SerializeField]
    Color noIconColor;

    [SerializeField]
    HPBar hpBar;

    Unit _unit;

    public void SetData(Unit unit)
    {
        _unit = unit;

        nameText.text = unit.Base.Name;
        lvlText.text = "Lvl " + unit.Level;
        hpBar.SetHP((float)unit.HP / unit.MaxHP);

        SetStatusIcon();
        unit.OnStatusChanged += SetStatusIcon;
    }

    void SetStatusIcon(){

        if(_unit.Status == null){
            conditionImage.sprite = null;
            conditionImage.color = noIconColor;
        }
        else if(_unit.Status.Name == "Gift"){
            conditionImage.sprite = poisonIcon;
            conditionImage.color = iconColor;
        }
        else if(_unit.Status.Name == "Søvn"){
            conditionImage.sprite = sleepIcon;
            conditionImage.color = iconColor;
        }
        else if(_unit.Status.Name == "Hjerneteppe"){
            conditionImage.sprite = stunIcon;
            conditionImage.color = iconColor;
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_unit.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_unit.HP / _unit.MaxHP);
            _unit.HpChanged = false;
        }
    }
}
