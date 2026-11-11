using UnityEngine;

// Handles tilling, planting, watering, and harvesting one farm plot.
public class FarmPlot : MonoBehaviour, IInteractive
{
    public enum PlotState { Untilled, Tilled, Planted }

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer soilRenderer;
    [SerializeField] private SpriteRenderer cropRenderer;
    [SerializeField] private Sprite untilledSprite;
    [SerializeField] private Sprite tilledSprite;
    [SerializeField] private Sprite wateredSprite;

    [Header("State")]
    [SerializeField] private PlotState state = PlotState.Untilled;

    private CropData plantedCrop;
    private int daysGrown;
    private bool wateredToday;

    public PlotState State => state;
    public bool CanPlant => state == PlotState.Tilled;
    public bool IsReadyToHarvest => plantedCrop != null && daysGrown >= plantedCrop.daysToGrow;

    // Subscribes to the new day event.
    private void OnEnable()
    {
        GameEvents.NewDay.AddListener(OnNewDay);
    }

    // Unsubscribes from the new day event.
    private void OnDisable()
    {
        GameEvents.NewDay.RemoveListener(OnNewDay);
    }

    // Applies the starting visuals.
    private void Start()
    {
        UpdateVisuals();
    }

    // Turns untilled ground into soil.
    public bool Till()
    {
        if (state != PlotState.Untilled) return false;
        state = PlotState.Tilled;
        UpdateVisuals();
        return true;
    }

    // Waters the plot once per day.
    public bool Water()
    {
        if (state == PlotState.Untilled) return false;
        if (wateredToday) return false;
        wateredToday = true;
        UpdateVisuals();
        return true;
    }

    // Plants a crop on tilled soil.
    public bool Plant(CropData crop)
    {
        if (state != PlotState.Tilled || crop == null) return false;
        plantedCrop = crop;
        daysGrown = 0;
        state = PlotState.Planted;
        UpdateVisuals();
        return true;
    }

    // Harvests the crop when it is ready.
    public void OnInteract()
    {
        if (!IsReadyToHarvest) return;
        if (PlayerInventory.Instance != null) PlayerInventory.Instance.AddItem(plantedCrop, plantedCrop.harvestYield);

        state = PlotState.Tilled;
        plantedCrop = null;
        daysGrown = 0;
        wateredToday = false;
        UpdateVisuals();
    }

    // Grows the crop if it was watered.
    private void OnNewDay(int day)
    {
        if (state == PlotState.Planted && wateredToday) daysGrown++;
        wateredToday = false;
        UpdateVisuals();
    }

    // Updates the soil and crop sprites.
    private void UpdateVisuals()
    {
        if (soilRenderer != null)
        {
            if (state == PlotState.Untilled) soilRenderer.sprite = untilledSprite;
            else soilRenderer.sprite = wateredToday ? wateredSprite : tilledSprite;
        }

        if (cropRenderer != null)
        {
            if (state == PlotState.Planted && plantedCrop != null) cropRenderer.sprite = plantedCrop.GetGrowthSprite(daysGrown);
            else cropRenderer.sprite = null;
        }
    }
}
