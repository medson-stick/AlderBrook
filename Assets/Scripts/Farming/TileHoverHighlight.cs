using UnityEngine;
using UnityEngine.InputSystem;

// Highlights the farm plot the player can currently use a tile tool on.
public class TileHoverHighlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private PlayerToolController toolController;

    [Header("Tile Detection")]
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private float targetRange = 1.5f;

    [Header("Rendering")]
    [SerializeField] private int sortingOrder = 100;

    public FarmPlot HoveredPlot { get; private set; }
    public FarmPlot ActivePlot { get; private set; }
    public bool HasActiveTile => ActivePlot != null;

    // Sets up the camera, placeholder visual, and hides the highlight.
    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        if (highlightRenderer == null)
        {
            GameObject highlightObject = new GameObject("PlaceholderHighlight");
            highlightObject.transform.SetParent(transform, false);
            highlightRenderer = highlightObject.AddComponent<SpriteRenderer>();
        }

        if (highlightRenderer.sprite == null)
        {
            highlightRenderer.sprite = CreatePlaceholderSprite();
        }

        highlightRenderer.sortingOrder = sortingOrder;
        highlightRenderer.enabled = false;
    }

    // Updates the active tile every frame.
    private void Update()
    {
        if (targetCamera == null || Mouse.current == null)
        {
            HideHighlight();
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = targetCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -targetCamera.transform.position.z));
        worldPosition.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(worldPosition, tileLayer);
        HoveredPlot = hit != null ? hit.GetComponent<FarmPlot>() : null;

        if (HoveredPlot == null ||
            toolController == null ||
            toolController.SelectedTool == null ||
            !toolController.SelectedTool.usesTileTargeting)
        {
            HideHighlight();
            return;
        }

        if (Vector2.Distance(toolController.transform.position, HoveredPlot.transform.position) > targetRange)
        {
            HideHighlight();
            return;
        }

        ActivePlot = HoveredPlot;
        ShowHighlight(HoveredPlot.transform.position);
    }

    // Shows the highlight on the active plot.
    private void ShowHighlight(Vector3 position)
    {
        if (highlightRenderer == null) return;

        highlightRenderer.transform.position = new Vector3(position.x, position.y, 0f);
        highlightRenderer.enabled = true;
    }

    // Hides the highlight and clears the hover data.
    private void HideHighlight()
    {
        HoveredPlot = null;
        ActivePlot = null;

        if (highlightRenderer != null) highlightRenderer.enabled = false;
    }

    // Builds a simple tile outline so the script works before art is added.
    private Sprite CreatePlaceholderSprite()
    {
        const int size = 32;
        const int border = 3;

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        Color clear = new Color(1f, 1f, 1f, 0f);
        Color outline = new Color(1f, 0.92f, 0.35f, 0.9f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isBorder = x < border || x >= size - border || y < border || y >= size - border;
                pixels[y * size + x] = isBorder ? outline : clear;
            }
        }

        texture.SetPixels(pixels);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
