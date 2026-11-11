using UnityEngine;

// Tool that waters tilled soil and crops.
[CreateAssetMenu(fileName = "WateringCanTool", menuName = "AlderBrook/Tools/Watering Can")]
public class WateringCanTool : ToolData
{
    // Waters the target plot.
    public override bool UseOn(FarmPlot plot)
    {
        if (plot == null) return false;
        return plot.Water();
    }
}
