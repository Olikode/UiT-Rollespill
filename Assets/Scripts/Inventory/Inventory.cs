using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;

    public event Action OnUpdated;

    public List<ItemSlot> Slots => slots;

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
    public ItemBase GetItem(int index)
    {
        return slots[index].Item;
    }

    public ItemBase UseItem(int itemIndex, Unit unit)
    {
        var item = slots[itemIndex].Item;
        bool itemUsed = item.Use(unit);

        if(itemUsed)
        {
            RemoveItem(item);
            return item;
        }
        
        return null;
    }

    public void RemoveItem(ItemBase item)
    {
        // decrease item count
        var itemSlot = slots.First(slots => slots.Item == item);
        itemSlot.Count--;

        // remove item from inventory when 0 left
        if(itemSlot.Count == 0)
            slots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }
}

[Serializable] public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item => item;
    public int Count {
        get => count;
        set => count = value;
    }
}
