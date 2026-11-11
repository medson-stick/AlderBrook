using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controls one row in the shop list.
public class ShopRowUI : MonoBehaviour
{
    [Header("Row UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private Button actionButton;

    // Sets up this row for buying.
    public void SetupForBuy(Shop shop, ItemData item, ShopUI shopUI)
    {
        SetDisplay(item, item.buyPrice);
        if (actionText != null) actionText.text = "Buy";
        if (actionButton == null) return;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() =>
        {
            shop.Buy(item);
            shopUI.Refresh();
        });
    }

    // Sets up this row for selling.
    public void SetupForSell(Shop shop, ItemData item, ShopUI shopUI)
    {
        SetDisplay(item, item.sellPrice);
        if (actionText != null) actionText.text = "Sell";
        if (actionButton == null) return;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() =>
        {
            shop.Sell(item);
            shopUI.Refresh();
        });
    }

    // Fills in the icon, name, and price.
    private void SetDisplay(ItemData item, int price)
    {
        if (icon != null) icon.sprite = item.icon;
        if (nameText != null) nameText.text = item.itemName;
        if (priceText != null) priceText.text = $"{price}g";
    }
}
