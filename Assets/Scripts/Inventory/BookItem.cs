using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new book item")]
public class BookItem : ItemBase
{

    // TODO make books contain new moves for the player to learn

    public override bool Use(Unit unit)
    {
        return true;
    }
}
