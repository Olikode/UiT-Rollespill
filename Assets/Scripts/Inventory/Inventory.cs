using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum ItemCategory {Items, Books, Keys}

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
        slots.Sort((x, y) => string.Compare(x.Item.Name, y.Item.Name));
        bookSlots.Sort((x, y) => string.Compare(x.Item.Name, y.Item.Name));
        keySlots.Sort((x, y) => string.Compare(x.Item.Name, y.Item.Name));

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

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Items;
        else if (item is BookItem)
            return ItemCategory.Books;
        else 
            return ItemCategory.Keys;
    }

    public void AddItem(ItemBase item, int count=1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);

        if(itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count,
            });
        }
        OnUpdated?.Invoke();
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
        
        // if item is not used
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

// contains item and count of item
[Serializable] public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item {
        get => item;
        set => item = value;
    }
    public int Count {
        get => count;
        set => count = value;
    }
}
