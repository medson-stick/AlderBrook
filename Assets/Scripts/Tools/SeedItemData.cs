using UnityEngine;

// Seed item that can also be used as a planting tool.
[CreateAssetMenu(fileName = "NewSeed", menuName = "AlderBrook/Seed")]
public class SeedItemData : ToolData
{
    public CropData crop;

    // Plants the crop and consumes one seed.
    public override bool UseOn(FarmPlot plot)
    {
        if (plot == null || crop == null) return false;
        if (!plot.CanPlant) return false;
        if (PlayerInventory.Instance == null) return false;
        if (!PlayerInventory.Instance.RemoveItem(this, 1)) return false;

        bool planted = plot.Plant(crop);
        if (!planted) PlayerInventory.Instance.AddItem(this, 1);
        return planted;
    }
}
