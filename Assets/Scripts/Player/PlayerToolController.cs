using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Handles hotbar slot selection and using the active item/tool on farm plots.
public class PlayerToolController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text selectedToolText;
    [SerializeField] private Camera targetCamera;

    [Header("Tool Targeting")]
    [SerializeField] private int selectedSlotIndex;

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

    // Caches the camera used to find the tile under the mouse.
    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
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

    // Uses the tool on the farm plot under the mouse.
    private void UseSelectedTool()
    {
        ToolData tool = SelectedTool;
        if (tool == null || !tool.usesTileTargeting) return;

        FarmPlot plot = FindPlotUnderMouse();
        if (plot != null) tool.UseOn(plot);
    }

    // Finds the farm plot collider under the mouse cursor.
    private FarmPlot FindPlotUnderMouse()
    {
        if (targetCamera == null || Mouse.current == null) return null;

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = targetCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -targetCamera.transform.position.z));
        worldPosition.z = 0f;

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);
        foreach (Collider2D hit in hits)
        {
            FarmPlot plot = hit.GetComponent<FarmPlot>();
            if (plot != null) return plot;
        }

        return null;
    }
}
