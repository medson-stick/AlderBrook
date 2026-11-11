using UnityEngine;

// Builds and controls the shop panel.
public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform buyContent;
    [SerializeField] private Transform sellContent;
    [SerializeField] private ShopRowUI rowPrefab;
    [SerializeField] private PlayerController playerController;

    private Shop activeShop;
    private float previousTimeScale = 1f;

    // Hides the shop panel at the start.
    private void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    // Opens the shop and pauses the game.
    public void Open(Shop shop)
    {
        if (activeShop != null) return;

        activeShop = shop;
        previousTimeScale = Time.timeScale;
        if (panel != null) panel.SetActive(true);
        if (playerController != null) playerController.SetInputEnabled(false);
        Time.timeScale = 0f;
        Refresh();
    }

    // Closes the shop and resumes the game.
    public void Close()
    {
        activeShop = null;
        if (panel != null) panel.SetActive(false);
        if (playerController != null) playerController.SetInputEnabled(true);
        Time.timeScale = previousTimeScale;
    }

    // Rebuilds the buy and sell lists.
    public void Refresh()
    {
        if (activeShop == null) return;
        if (rowPrefab == null) return;

        ClearChildren(buyContent);
        foreach (ShopStock entry in activeShop.Stock)
        {
            if (entry.item == null || entry.stockAmount <= 0) continue;
            ShopRowUI row = Instantiate(rowPrefab, buyContent);
            row.SetupForBuy(activeShop, entry.item, this);
        }

        ClearChildren(sellContent);
        if (PlayerInventory.Instance != null)
        {
            foreach (ItemStack stack in PlayerInventory.Instance.Items)
            {
                if (stack.item == null || stack.amount <= 0) continue;
                ShopRowUI row = Instantiate(rowPrefab, sellContent);
                row.SetupForSell(activeShop, stack.item, this);
            }
        }
    }

    // Removes old rows before rebuilding.
    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }
}
