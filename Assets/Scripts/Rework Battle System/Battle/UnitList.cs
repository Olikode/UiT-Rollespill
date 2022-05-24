using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitList : MonoBehaviour
{
    [SerializeField]
    List<Unit> units;

    private void Start()
    {
        foreach (var unit in units)
        {
            unit.Init();
        }
    }

    // get the first healthy unit from challenger
    public Unit GetHealthyUnit()
    {
        return units.Where(x => x.HP > 0).FirstOrDefault();
    }

    // get player unit
    // player will only have a list of 1 unit 
    public Unit GetPlayerUnit()
    {
        return units[0];
    }

    public void SetPlayerUnit(UnitBase unitBase)
    {
        var unit = new Unit();
        unit.SetPlayerData(unitBase);

        units.Add(unit);
    }
}
