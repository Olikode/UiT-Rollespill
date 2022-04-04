using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB : MonoBehaviour
{
    public static Dictionary<ConditionID, Condition> Conditions {get; set;} 
    = new Dictionary<ConditionID, Condition>() {
        {
            ConditionID.Gift,
            new Condition(){
                Name = "Forgiftet",
                StartMessage ="Har blitt forgiftet",
                OnAfterTurn = (Unit unit) =>
                {
                    unit.UpdateHP(unit.MaxHP / 8);
                    unit.StatusChanges.Enqueue($"{unit.Base.Name} ble skadet av giften");
                }
            }
        },
        {
            ConditionID.Hjerneteppe,
            new Condition(){
                Name = "Hjerneteppe",
                StartMessage ="Har fått hjerneteppe",
                OnStart = (Unit unit) =>
                {
                    unit.StatusTime = Random.Range(2,6);
                    Debug.Log("Status time: " + unit.StatusTime);
                },
                OnBeforeMove = (Unit unit) =>
                {
                    unit.StatusTime --;
                    Debug.Log("Status time decreased: " + unit.StatusTime);
                    if (unit.StatusTime <= 0){
                        unit.CureStatus();
                        unit.StatusChanges.Enqueue($"{unit.Base.Name} har ikke hjerneteppe lengre");
                        return true;
                    }

                    if (Random.Range(1,3) == 1){
                    unit.StatusChanges.Enqueue($"{unit.Base.Name} har hjerneteppe og klarer ikke tenke");
                    return false;
                    }
                    return true;
                }
            }
        },
    };
}

public enum ConditionID{
    Null, Gift, Søvn, Hjerneteppe
}
