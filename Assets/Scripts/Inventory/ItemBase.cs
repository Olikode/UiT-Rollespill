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


    public string Name => name;
    public string Description => description;
    public string UseMessage => useMessage;
    public Sprite Icon => icon;

    public virtual bool Use(Unit unit)
    {
        return false;
    }
}
