using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new book item")]
public class BookItem : ItemBase
{
    public override bool Use(Unit unit)
    {
        return true;
    }
}
