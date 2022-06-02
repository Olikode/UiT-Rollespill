using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new key")]
public class KeyItem : ItemBase
{
    //Should be used as quest items to unlock new places or finish quests

    public override bool UsableInBattle => false;
    public override bool UsableOutsideBattle => false;
}
