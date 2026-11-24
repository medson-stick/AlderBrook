using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Handles hotbar slot selection and using the active item/tool on farm plots.
public class PlayerToolController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TMP_Text selectedToolText;
    [SerializeField] private TileHoverHighlight tileHighlight;

    [Header("Tool Targeting")]
    [SerializeField] private int selectedSlotIndex;
    [SerializeField] private float toolOffset = 0.75f;
    [SerializeField] private float toolRadius = 0.45f;
    [SerializeField] private LayerMask interactableLayer;

    public int SelectedSlotIndex => selectedSlotIndex;

    // The active slot's item only works as a tool when it is ToolData.
    public ToolData SelectedTool
    {
        get
        {
            if (PlayerInventory.Instance == null) return null;
            return PlayerInventory.Instance.GetItem(selectedSlotIndex) as ToolData;
        }
    }

    // Shows the starting hotbar item name.
    private void Start()
    {
        SelectSlot(selectedSlotIndex);
    }

    // Uses the selected tool when Attack is pressed.
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        if (InventoryUI.Instance != null && InventoryUI.Instance.IsOpen) return;
        UseSelectedTool();
    }

    // Makes a hotbar slot active and refreshes the tool name label.
    public void SelectSlot(int index)
    {
        selectedSlotIndex = Mathf.Clamp(index, 0, PlayerInventory.HotbarSize - 1);
        UpdateToolLabel();
    }

    // Cycles through the hotbar row with wrap-around.
    public void CycleSlot(int direction)
    {
        int next = (selectedSlotIndex + direction + PlayerInventory.HotbarSize) % PlayerInventory.HotbarSize;
        SelectSlot(next);
    }

    // Updates the optional active item name label.
    private void UpdateToolLabel()
    {
        if (selectedToolText == null) return;

        ItemData item = PlayerInventory.Instance != null ? PlayerInventory.Instance.GetItem(selectedSlotIndex) : null;
        selectedToolText.text = item != null ? item.itemName : string.Empty;
    }

    // Uses the highlighted plot when possible, otherwise falls back to the facing tile.
    private void UseSelectedTool()
    {
        ToolData tool = SelectedTool;
        if (tool == null || !tool.usesTileTargeting) return;

        if (tileHighlight != null && tileHighlight.HasActiveTile)
        {
            tool.UseOn(tileHighlight.ActivePlot);
            return;
        }

        if (playerController == null) return;

        Vector2 targetPosition = (Vector2)playerController.transform.position + playerController.FacingDirection * toolOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, toolRadius, interactableLayer);

        foreach (Collider2D hit in hits)
        {
            FarmPlot plot = hit.GetComponent<FarmPlot>();
            if (plot == null) continue;

            tool.UseOn(plot);
            return;
        }
    }
}
