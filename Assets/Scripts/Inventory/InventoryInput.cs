using UnityEngine;
using UnityEngine.InputSystem;

// Receives PlayerInput action messages (Send Messages behavior) for inventory and hotbar input.
public class InventoryInput : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PlayerToolController toolController;

    private int lastScrollFrame = -1;

    // Tab toggles the inventory panel. PlayerInput only sends "performed" for buttons.
    public void OnToggleInventory(InputValue value)
    {
        if (value.isPressed && inventoryUI != null) inventoryUI.Toggle();
    }

    public void OnHotbar1(InputValue value) { SelectHotbar(0, value); }
    public void OnHotbar2(InputValue value) { SelectHotbar(1, value); }
    public void OnHotbar3(InputValue value) { SelectHotbar(2, value); }
    public void OnHotbar4(InputValue value) { SelectHotbar(3, value); }
    public void OnHotbar5(InputValue value) { SelectHotbar(4, value); }
    public void OnHotbar6(InputValue value) { SelectHotbar(5, value); }
    public void OnHotbar7(InputValue value) { SelectHotbar(6, value); }
    public void OnHotbar8(InputValue value) { SelectHotbar(7, value); }
    public void OnHotbar9(InputValue value) { SelectHotbar(8, value); }
    public void OnHotbar0(InputValue value) { SelectHotbar(9, value); }

    // Mouse wheel cycles the hotbar while the inventory is closed.
    public void OnHotbarScroll(InputValue value)
    {
        if (inventoryUI == null || toolController == null || inventoryUI.IsOpen) return;
        if (lastScrollFrame == Time.frameCount) return;

        Vector2 scroll = value.Get<Vector2>();
        if (Mathf.Abs(scroll.y) < 0.01f) return;

        lastScrollFrame = Time.frameCount;
        toolController.CycleSlot(scroll.y > 0f ? -1 : 1);
        inventoryUI.Refresh();
    }

    // Selects a hotbar slot when its number key is pressed.
    private void SelectHotbar(int index, InputValue value)
    {
        if (value.isPressed && inventoryUI != null) inventoryUI.SelectHotbar(index);
    }
}
