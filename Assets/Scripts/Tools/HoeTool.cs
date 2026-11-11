using UnityEngine;

// Tool that tills untilled soil.
[CreateAssetMenu(fileName = "HoeTool", menuName = "AlderBrook/Tools/Hoe")]
public class HoeTool : ToolData
{
    // Tills the target plot.
    public override bool UseOn(FarmPlot plot)
    {
        if (plot == null) return false;
        return plot.Till();
    }
}
