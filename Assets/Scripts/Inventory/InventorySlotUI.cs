using UnityEngine;
using UnityEngine.EventSystems;

// Draws a single inventory slot and forwards clicks/drags to the owning InventoryUI.
// Clicking picks up/places a stack; dragging moves it and drops it on release.
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private UnityEngine.UI.Image background;
    [SerializeField] private UnityEngine.UI.Image icon;
    [SerializeField] private TMPro.TMP_Text amountText;
    [SerializeField] private UnityEngine.UI.Image selection;

    [Header("Colors")]
    [SerializeField] private Color emptyColor = new Color(0.18f, 0.14f, 0.1f, 0.9f);
    [SerializeField] private Color filledColor = new Color(0.32f, 0.26f, 0.18f, 0.95f);
    [SerializeField] private Color selectionColor = new Color(1f, 0.87f, 0.4f, 0.9f);

    private InventoryUI inventoryUI;
    private int slotIndex = -1;

    public int SlotIndex => slotIndex;

    // Links this slot to its inventory index and owning UI.
    public void Bind(int index, InventoryUI owner)
    {
        slotIndex = index;
        inventoryUI = owner;
    }

    // Enables or disables mouse clicking/dragging on this slot.
    public void SetClickable(bool clickable)
    {
        if (background != null) background.raycastTarget = clickable;
    }

    // Click-to-move: pick up when nothing is held, otherwise place/swap.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryUI != null) inventoryUI.HandleSlotClicked(slotIndex);
    }

    // Drag-and-drop: pick up the stack when a drag starts.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryUI != null) inventoryUI.BeginDrag(slotIndex);
    }

    // Required so the EventSystem treats this slot as draggable; the visual follows the mouse.
    public void OnDrag(PointerEventData eventData) { }

    // Drag-and-drop: drop the held stack on whatever slot is under the cursor.
    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventoryUI == null) return;

        InventorySlotUI target = null;
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            target = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<InventorySlotUI>();
        }

        inventoryUI.EndDrag(target != null ? target.SlotIndex : -1);
    }

    // Paints the slot from a stack and shows whether it is the active hotbar slot.
    public void Refresh(ItemStack stack, bool isSelected)
    {
        bool hasItem = stack != null && stack.item != null && stack.amount > 0;

        if (icon != null)
        {
            icon.sprite = hasItem ? stack.item.icon : null;
            icon.enabled = hasItem && icon.sprite != null;
        }

        if (amountText != null)
        {
            amountText.text = hasItem && stack.amount > 1 ? stack.amount.ToString() : string.Empty;
        }

        if (background != null) background.color = hasItem ? filledColor : emptyColor;

        if (selection != null)
        {
            selection.color = selectionColor;
            selection.enabled = isSelected;
        }
    }
}
