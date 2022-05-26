using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new move book")]
public class BookItem : ItemBase
{
    [SerializeField] MoveBase move;


    public override bool Use(Unit unit)
    {
        // learning move is handled in InventoruUI
        // returns true if used
        return unit.HasMove(move);
    }

    public override bool UsableInBattle => false;

    public MoveBase Move => move;
}
