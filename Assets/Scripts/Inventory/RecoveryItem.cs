using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP recovery")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP recovery")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;


    [Header("Condition recovery")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAnyStatus;

    public override bool Use(Unit unit)
    {
        if (hpAmount > 0)
        {
            if (unit.HP == unit.MaxHP)
                return false;
            
            unit.IncreaseHP(hpAmount);
        }
        if (ppAmount > 0)
        {
           /* if (unit.HP == unit.MaxHP)
                return false;
            
            unit.IncreaseHP(hpAmount);*/
        }

        return true;
    }
}
