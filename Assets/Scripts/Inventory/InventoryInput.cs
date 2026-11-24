using UnityEngine;
using UnityEngine.InputSystem;

// Reads Tab for the inventory and the number row / mouse wheel for hotbar selection.
public class InventoryInput : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PlayerToolController toolController;

    // Finds the UI and tool controller when they are not assigned in the inspector.
    private void Awake()
    {
        if (inventoryUI == null) inventoryUI = FindFirstObjectByType<InventoryUI>();
        if (toolController == null) toolController = FindFirstObjectByType<PlayerToolController>();
    }

    // Polls keyboard and mouse every frame for inventory and hotbar input.
    private void Update()
    {
        if (inventoryUI == null || Keyboard.current == null) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame) inventoryUI.Toggle();

        if (inventoryUI.IsOpen && Keyboard.current.escapeKey.wasPressedThisFrame) inventoryUI.Close();

        for (int i = 0; i < PlayerInventory.HotbarSize; i++)
        {
            if (Keyboard.current[GetHotbarKey(i)].wasPressedThisFrame) inventoryUI.SelectHotbar(i);
        }

        if (!inventoryUI.IsOpen && toolController != null && Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                toolController.CycleSlot(scroll > 0f ? -1 : 1);
                inventoryUI.Refresh();
            }
        }
    }

    // Maps hotbar index 0-9 to the number keys 1-9 and 0.
    private static Key GetHotbarKey(int index)
    {
        switch (index)
        {
            case 0: return Key.Digit1;
            case 1: return Key.Digit2;
            case 2: return Key.Digit3;
            case 3: return Key.Digit4;
            case 4: return Key.Digit5;
            case 5: return Key.Digit6;
            case 6: return Key.Digit7;
            case 7: return Key.Digit8;
            case 8: return Key.Digit9;
            default: return Key.Digit0;
        }
    }
}
