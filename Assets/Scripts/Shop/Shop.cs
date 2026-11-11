using System.Collections.Generic;
using UnityEngine;

// One item that a shop sells.
[System.Serializable]
public class ShopStock
{
    public ItemData item;
    public int stockAmount = 5;
}

// Handles buying and selling at a shop.
public class Shop : MonoBehaviour, IInteractive
{
    [Header("Shop")]
    [SerializeField] private string shopName = "General Store";
    [SerializeField] private List<ShopStock> stock = new List<ShopStock>();
    [SerializeField] private ShopUI shopUI;

    public string ShopName => shopName;
    public List<ShopStock> Stock => stock;

    // Opens the shop UI when the player interacts.
    public void OnInteract()
    {
        if (shopUI != null) shopUI.Open(this);
    }

    // Buys one item if it is in stock and the player can afford it.
    public bool Buy(ItemData item)
    {
        ShopStock stockEntry = FindStock(item);
        if (stockEntry == null || stockEntry.stockAmount <= 0) return false;
        if (PlayerWallet.Instance == null || PlayerInventory.Instance == null) return false;
        if (!PlayerWallet.Instance.SpendMoney(item.buyPrice)) return false;

        stockEntry.stockAmount--;
        PlayerInventory.Instance.AddItem(item, 1);
        return true;
    }

    // Sells one item from the player's inventory.
    public bool Sell(ItemData item)
    {
        if (PlayerWallet.Instance == null || PlayerInventory.Instance == null) return false;
        if (!PlayerInventory.Instance.RemoveItem(item, 1)) return false;

        PlayerWallet.Instance.AddMoney(item.sellPrice);
        return true;
    }

    // Finds the stock entry for an item.
    private ShopStock FindStock(ItemData item)
    {
        foreach (ShopStock entry in stock)
        {
            if (entry.item == item) return entry;
        }
        return null;
    }
}
