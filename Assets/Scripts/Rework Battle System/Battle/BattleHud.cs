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
    HPBar hpBar;

    Unit _unit;

    public void SetData(Unit unit)
    {
        _unit = unit;

        nameText.text = unit.Base.Name;
        lvlText.text = "Lvl " + unit.Level;
        hpBar.SetHP((float)unit.HP / unit.MaxHP);
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
