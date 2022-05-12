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
    [SerializeField] bool recoverAllStatus;

    public override bool Use(Unit unit)
    {
        // restore health
        if (restoreMaxHP || hpAmount > 0)
        {
            // check if player is not already full at max HP 
            if (unit.HP == unit.MaxHP)
                return false;

            if(restoreMaxHP)
                unit.IncreaseHP(unit.MaxHP);
            else
                unit.IncreaseHP(hpAmount);
        }

        // recover status condition
        if (recoverAllStatus || status != ConditionID.Null)
        {
            // check if player has status condition
            if(unit.Status == null)
                return false;

            if(recoverAllStatus)
                unit.CureStatus();
            else
            {
                if (unit.Status.Id == status)
                    unit.CureStatus();
                else
                    return false;
            }
        }

        // restore PP
        if (restoreMaxPP)
           unit.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        else if (ppAmount > 0)
            unit.Moves.ForEach(m => m.IncreasePP(ppAmount));

        return true;
    }
}
