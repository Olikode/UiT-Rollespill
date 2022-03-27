using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField]
    UnitBase _base;

    [SerializeField]
    int level;

    [SerializeField]
    bool isPlayer;

    public Unit1 Unit { get; set; }

    public void Setup()
    {
        Unit = new Unit1(_base, level);
        GetComponent<Image>().sprite = Unit.Base.Sprite;
    }
}
