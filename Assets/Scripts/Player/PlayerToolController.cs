using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Handles tool selection and using tools on farm plots.
public class PlayerToolController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ToolData[] tools;
    [SerializeField] private TMP_Text selectedToolText;
    [SerializeField] private TileHoverHighlight tileHighlight;

    [Header("Tool Targeting")]
    [SerializeField] private float toolOffset = 0.75f;
    [SerializeField] private float toolRadius = 0.45f;
    [SerializeField] private LayerMask interactableLayer;

    private int selectedToolIndex;

    public ToolData SelectedTool
    {
        get
        {
            if (tools == null || tools.Length == 0) return null;
            return tools[selectedToolIndex];
        }
    }

    // Shows the starting tool name.
    private void Start()
    {
        UpdateToolLabel();
    }

    // Uses the selected tool when Attack is pressed.
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        UseSelectedTool();
    }

    // Selects the next tool.
    public void OnNext(InputValue value)
    {
        if (!value.isPressed) return;
        CycleTool(1);
    }

    // Selects the previous tool.
    public void OnPrevious(InputValue value)
    {
        if (!value.isPressed) return;
        CycleTool(-1);
    }

    // Changes the selected tool index with wrap-around.
    private void CycleTool(int direction)
    {
        if (tools == null || tools.Length == 0) return;

        selectedToolIndex = (selectedToolIndex + direction + tools.Length) % tools.Length;
        if (SelectedTool != null) Debug.Log($"Selected tool: {SelectedTool.itemName}");
        UpdateToolLabel();
    }

    // Updates the optional tool name label.
    private void UpdateToolLabel()
    {
        if (selectedToolText == null) return;
        selectedToolText.text = SelectedTool != null ? SelectedTool.itemName : "No Tool";
    }

    // Uses the highlighted plot when possible, otherwise falls back to the facing tile.
    private void UseSelectedTool()
    {
        if (SelectedTool == null) return;
        if (!SelectedTool.usesTileTargeting) return;

        if (tileHighlight != null && tileHighlight.HasActiveTile)
        {
            SelectedTool.UseOn(tileHighlight.ActivePlot);
            return;
        }

        if (playerController == null) return;

        Vector2 targetPosition = (Vector2)playerController.transform.position + playerController.FacingDirection * toolOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, toolRadius, interactableLayer);

        foreach (Collider2D hit in hits)
        {
            FarmPlot plot = hit.GetComponent<FarmPlot>();
            if (plot == null) continue;

            SelectedTool.UseOn(plot);
            return;
        }
    }
}
