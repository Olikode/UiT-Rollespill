using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public void Heal()
    {
        var unitList = GameController.playerUnit;
        unitList.Units.ForEach(u => u.Heal());
    }
}
