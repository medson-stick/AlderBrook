using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Displays the player's 3x10 inventory grid and the always-visible hotbar row.
// Slot visuals are authored in the scene; this fills them and handles moving items.
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform hotbarRoot;
    [SerializeField] private Transform gridRoot;
    [SerializeField] private PlayerToolController toolController;

    [Header("Held Item")]
    [SerializeField] private GameObject heldItemRoot;
    [SerializeField] private Image heldItemIcon;
    [SerializeField] private TMP_Text heldItemAmount;

    private InventorySlotUI[] hotbarSlots;
    private InventorySlotUI[] gridSlots;
    private RectTransform heldItemRect;
    private ItemStack heldStack;
    private bool isOpen;

    public bool IsOpen => isOpen;
    public bool IsHolding => heldStack != null;

    // Caches scene slots, links them to inventory indices, and prepares the held visual.
    private void Awake()
    {
        Instance = this;

        if (hotbarRoot != null)
        {
            hotbarSlots = hotbarRoot.GetComponentsInChildren<InventorySlotUI>(true);
            for (int i = 0; i < hotbarSlots.Length; i++) hotbarSlots[i].Bind(i, this);
        }

        if (gridRoot != null)
        {
            gridSlots = gridRoot.GetComponentsInChildren<InventorySlotUI>(true);
            for (int i = 0; i < gridSlots.Length; i++) gridSlots[i].Bind(i, this);
        }

        if (heldItemRoot != null) heldItemRect = heldItemRoot.GetComponent<RectTransform>();
        UpdateHeldVisual();
    }

    // Subscribes to inventory changes and hides the full grid at startup.
    private void Start()
    {
        if (PlayerInventory.Instance != null) PlayerInventory.Instance.onInventoryChanged.AddListener(Refresh);

        Close();
        Refresh();
    }

    // Removes the inventory listener when the UI is destroyed.
    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null) PlayerInventory.Instance.onInventoryChanged.RemoveListener(Refresh);
    }

    // Keeps the held-item visual under the mouse.
    private void Update()
    {
        UpdateHeldVisual();
    }

    // Opens the inventory when closed, closes it when open.
    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    // Shows the full inventory grid without stopping player movement.
    public void Open()
    {
        if (isOpen) return;
        if (Time.timeScale == 0f) return;

        isOpen = true;
        if (panel != null) panel.SetActive(true);
        SetSlotsClickable(true);
        Refresh();
    }

    // Hides the full inventory grid and returns any item still being moved.
    public void Close()
    {
        if (heldStack != null) ReturnHeldStack();

        isOpen = false;
        if (panel != null) panel.SetActive(false);
        SetSlotsClickable(false);
        UpdateHeldVisual();
    }

    // Makes a hotbar slot the active item and repaints the slots.
    public void SelectHotbar(int index)
    {
        if (toolController != null) toolController.SelectSlot(index);
        Refresh();
    }

    // Click-to-move: pick up, place, or swap the clicked stack.
    public void HandleSlotClicked(int index)
    {
        if (!isOpen || PlayerInventory.Instance == null) return;

        if (heldStack == null)
        {
            ItemStack stack = PlayerInventory.Instance.GetStack(index);
            if (stack == null || stack.item == null) return;

            heldStack = PlayerInventory.Instance.TakeStack(index);
        }
        else
        {
            PlaceHeldInto(index);
        }

        Refresh();
        UpdateHeldVisual();
    }

    // Drag-and-drop: called when a drag starts on a slot.
    public void BeginDrag(int index)
    {
        if (!isOpen || PlayerInventory.Instance == null || heldStack != null) return;

        ItemStack stack = PlayerInventory.Instance.GetStack(index);
        if (stack == null || stack.item == null) return;

        heldStack = PlayerInventory.Instance.TakeStack(index);
        Refresh();
        UpdateHeldVisual();
    }

    // Drag-and-drop: called when the mouse is released. Target index is -1 when not over a slot.
    public void EndDrag(int targetIndex)
    {
        if (heldStack == null) return;

        if (targetIndex < 0) ReturnHeldStack();
        else PlaceHeldInto(targetIndex);

        Refresh();
        UpdateHeldVisual();
    }

    // Repaints every hotbar and grid slot from the current inventory data.
    public void Refresh()
    {
        if (PlayerInventory.Instance == null) return;

        if (toolController != null) toolController.SelectSlot(toolController.SelectedSlotIndex);
        int selected = toolController != null ? toolController.SelectedSlotIndex : 0;

        if (hotbarSlots != null)
        {
            for (int i = 0; i < hotbarSlots.Length && i < PlayerInventory.HotbarSize; i++)
            {
                hotbarSlots[i].Refresh(PlayerInventory.Instance.GetStack(i), i == selected);
            }
        }

        if (gridSlots != null)
        {
            for (int i = 0; i < gridSlots.Length && i < PlayerInventory.SlotCount; i++)
            {
                gridSlots[i].Refresh(PlayerInventory.Instance.GetStack(i), i == selected);
            }
        }
    }

    // Places the held stack into a slot, swapping with anything already there.
    private void PlaceHeldInto(int index)
    {
        if (heldStack == null || PlayerInventory.Instance == null) return;

        ItemStack target = PlayerInventory.Instance.GetStack(index);

        if (target == null)
        {
            PlayerInventory.Instance.SetStack(index, heldStack);
            heldStack = null;
        }
        else
        {
            PlayerInventory.Instance.SetStack(index, heldStack);
            heldStack = target;
        }
    }

    // Enables or disables mouse clicking/dragging on every slot.
    private void SetSlotsClickable(bool clickable)
    {
        if (hotbarSlots != null)
        {
            foreach (InventorySlotUI slot in hotbarSlots) slot.SetClickable(clickable);
        }

        if (gridSlots != null)
        {
            foreach (InventorySlotUI slot in gridSlots) slot.SetClickable(clickable);
        }
    }

    // Puts a held stack back into the inventory so closing never loses items.
    private void ReturnHeldStack()
    {
        if (heldStack == null) return;
        if (PlayerInventory.Instance != null) PlayerInventory.Instance.AddItem(heldStack.item, heldStack.amount);
        heldStack = null;
    }

    // Keeps the dragged item centered on the mouse while a stack is held.
    private void UpdateHeldVisual()
    {
        if (heldItemRoot == null) return;

        bool holding = heldStack != null && heldStack.item != null;

        if (holding && !heldItemRoot.activeSelf) heldItemRoot.transform.SetAsLastSibling();
        heldItemRoot.SetActive(holding);
        if (!holding) return;

        if (heldItemIcon != null)
        {
            heldItemIcon.sprite = heldStack.item.icon;
            heldItemIcon.enabled = heldItemIcon.sprite != null;
        }

        if (heldItemAmount != null)
        {
            heldItemAmount.text = heldStack.amount > 1 ? heldStack.amount.ToString() : string.Empty;
        }

        if (heldItemRect != null && Mouse.current != null && transform is RectTransform)
        {
            RectTransform canvasRect = (RectTransform)transform;
            Canvas canvas = canvasRect.GetComponent<Canvas>();
            Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;

            Vector2 screen = Mouse.current.position.ReadValue();
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screen, eventCamera, out Vector3 worldPoint))
            {
                heldItemRect.position = worldPoint;
            }
        }
    }
}
