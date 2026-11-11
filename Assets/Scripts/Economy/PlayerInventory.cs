using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Holds an item and how many of it the player has.
[System.Serializable]
public class ItemStack
{
    public ItemData item;
    public int amount;
}

// Stores and manages the player's items.
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [SerializeField] private List<ItemStack> items = new List<ItemStack>();
    public UnityEvent onInventoryChanged = new UnityEvent();

    public List<ItemStack> Items => items;

    // Keeps only one inventory alive.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Adds an item and merges it with an existing stack.
    public void AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return;

        ItemStack existingStack = FindStack(item);
        if (existingStack != null) existingStack.amount += amount;
        else items.Add(new ItemStack { item = item, amount = amount });

        onInventoryChanged.Invoke();
    }

    // Removes items if the player has enough.
    public bool RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;

        ItemStack existingStack = FindStack(item);
        if (existingStack == null || existingStack.amount < amount) return false;

        existingStack.amount -= amount;
        if (existingStack.amount <= 0) items.Remove(existingStack);

        onInventoryChanged.Invoke();
        return true;
    }

    // Returns how many of an item the player has.
    public int GetAmount(ItemData item)
    {
        if (item == null) return 0;
        ItemStack existingStack = FindStack(item);
        return existingStack == null ? 0 : existingStack.amount;
    }

    // Checks whether the player has at least the requested amount.
    public bool HasItem(ItemData item, int amount)
    {
        return GetAmount(item) >= amount;
    }

    // Finds the stack for a specific item.
    private ItemStack FindStack(ItemData item)
    {
        foreach (ItemStack stack in items)
        {
            if (stack.item == item) return stack;
        }
        return null;
    }
}
