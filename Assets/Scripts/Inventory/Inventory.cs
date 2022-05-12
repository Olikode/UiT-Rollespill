using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> bookSlots;
    [SerializeField] List<ItemSlot> keySlots;

    List<List<ItemSlot>> allSlots;

    public static List<string> ItemCategories { get; set;} = new List<string>()
    {
        "ITEMS", "BØKER", "NØKLER",
    };

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, bookSlots, keySlots};
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }

    public ItemBase UseItem(int itemIndex, int categoryIndex, Unit unit)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);

        var item = currentSlots[itemIndex].Item;
        bool itemUsed = item.Use(unit);

        if(itemUsed)
        {
            RemoveItem(item, categoryIndex);
            return item;
        }
        
        return null;
    }

    public void RemoveItem(ItemBase item, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);

        // decrease item count
        var itemSlot = currentSlots.First(slots => slots.Item == item);
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
