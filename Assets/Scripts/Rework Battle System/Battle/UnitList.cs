using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitList : MonoBehaviour
{
    [SerializeField]
    List<Unit> units;

    public List<Unit> Units 
    { 
        get {return units;}
    }

    private void Start()
    {
        foreach (var unit in units)
        {
            unit.Init();
        }
    }

    public void AddPlayerUnit(UnitBase unitbase)
    {
        var playerUnit = new Unit();
        playerUnit.SetData(unitbase, 1);
        playerUnit.Init();

        units.Add(playerUnit);
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
}
