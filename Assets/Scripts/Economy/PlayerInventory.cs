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

// Stores and manages the player's items in a fixed 3x10 grid.
// The first row (slots 0-9) is also the hotbar.
public class PlayerInventory : MonoBehaviour
{
    public const int Columns = 10;
    public const int Rows = 3;
    public const int SlotCount = Columns * Rows;
    public const int HotbarSize = Columns;

    public static PlayerInventory Instance { get; private set; }

    [Header("Starting Items")]
    [SerializeField] private List<ItemStack> startingItems = new List<ItemStack>();

    public UnityEvent onInventoryChanged = new UnityEvent();

    private ItemStack[] slots = new ItemStack[SlotCount];
    private readonly List<ItemStack> nonEmptyItems = new List<ItemStack>();

    // Every stack that currently holds at least one item.
    public List<ItemStack> Items
    {
        get
        {
            nonEmptyItems.Clear();
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null && slots[i].amount > 0) nonEmptyItems.Add(slots[i]);
            }
            return nonEmptyItems;
        }
    }

    // Keeps only one inventory alive and fills the starting slots.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        slots = new ItemStack[SlotCount];
        int index = 0;
        foreach (ItemStack stack in startingItems)
        {
            if (stack == null || stack.item == null || stack.amount <= 0) continue;
            if (index >= SlotCount) break;

            slots[index] = new ItemStack { item = stack.item, amount = stack.amount };
            index++;
        }
    }

    // Notifies listeners once the starting items have been placed.
    private void Start()
    {
        onInventoryChanged.Invoke();
    }

    // Returns the stack stored in a slot, or null when the slot is empty.
    public ItemStack GetStack(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index];
    }

    // Returns the item stored in a slot, or null when the slot is empty.
    public ItemData GetItem(int index)
    {
        ItemStack stack = GetStack(index);
        return stack == null ? null : stack.item;
    }

    // Returns how many items are stored in a specific slot.
    public int GetAmount(int index)
    {
        ItemStack stack = GetStack(index);
        return stack == null ? 0 : stack.amount;
    }

    // Adds items to an existing stack of the same item, then to the first empty slot.
    public bool AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;

        for (int i = 0; i < slots.Length; i++)
        {
            ItemStack stack = slots[i];
            if (stack != null && stack.item == item)
            {
                stack.amount += amount;
                onInventoryChanged.Invoke();
                return true;
            }
        }

        int emptyIndex = FindEmptySlot();
        if (emptyIndex < 0) return false;

        slots[emptyIndex] = new ItemStack { item = item, amount = amount };
        onInventoryChanged.Invoke();
        return true;
    }

    // Removes items if the inventory holds at least the requested amount.
    public bool RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;
        if (GetAmount(item) < amount) return false;

        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            ItemStack stack = slots[i];
            if (stack == null || stack.item != item) continue;

            int removed = Mathf.Min(stack.amount, amount);
            stack.amount -= removed;
            amount -= removed;
            if (stack.amount <= 0) slots[i] = null;
        }

        onInventoryChanged.Invoke();
        return true;
    }

    // Removes items from one specific slot.
    public bool RemoveFromSlot(int index, int amount)
    {
        ItemStack stack = GetStack(index);
        if (stack == null || amount <= 0 || stack.amount < amount) return false;

        stack.amount -= amount;
        if (stack.amount <= 0) slots[index] = null;

        onInventoryChanged.Invoke();
        return true;
    }

    // Removes and returns the whole stack in a slot, or null when empty.
    public ItemStack TakeStack(int index)
    {
        if (index < 0 || index >= slots.Length) return null;

        ItemStack stack = slots[index];
        if (stack == null) return null;

        slots[index] = null;
        onInventoryChanged.Invoke();
        return stack;
    }

    // Places a stack into a slot, replacing whatever was there.
    public void SetStack(int index, ItemStack stack)
    {
        if (index < 0 || index >= slots.Length) return;

        slots[index] = stack;
        onInventoryChanged.Invoke();
    }

    // Returns the total amount of an item across every slot.
    public int GetAmount(ItemData item)
    {
        if (item == null) return 0;

        int total = 0;
        foreach (ItemStack stack in slots)
        {
            if (stack != null && stack.item == item) total += stack.amount;
        }
        return total;
    }

    // Checks whether the inventory holds at least the requested amount.
    public bool HasItem(ItemData item, int amount)
    {
        return GetAmount(item) >= amount;
    }

    // Finds the first slot without an item.
    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null || slots[i].amount <= 0) return i;
        }
        return -1;
    }
}
