using UnityEngine;

// Base class for tools. Tools are items, so they share item data.
public abstract class ToolData : ItemData
{
    [Header("Targeting")]
    public bool usesTileTargeting = false;

    // Applies the tool to a farm plot.
    public abstract bool UseOn(FarmPlot plot);
}
