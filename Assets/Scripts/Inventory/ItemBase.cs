using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [Header("Item Base")]
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] string useMessage;
    [SerializeField] Sprite icon;

    // TODO add selling price and buying price
    [Header("Price")]
    [SerializeField] int buyingPrice;
    [SerializeField] int sellingPrice;

    public string Name => name;
    public string Description => description;
    public string UseMessage => useMessage;
    public Sprite Icon => icon;
    public int BuyingPrice => buyingPrice;
    public int SellingPrice => sellingPrice;

    public virtual bool Use(Unit unit)
    {
        return false;
    }
}
