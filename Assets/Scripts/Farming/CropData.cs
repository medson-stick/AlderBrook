using UnityEngine;

// Stores crop growth data. It also acts as the harvested item.
[CreateAssetMenu(fileName = "NewCrop", menuName = "AlderBrook/Crop")]
public class CropData : ItemData
{
    public Sprite[] growthStageSprites;
    public int daysToGrow = 4;
    public int harvestYield = 1;

    // Returns the sprite for the current growth stage.
    public Sprite GetGrowthSprite(int daysGrown)
    {
        if (growthStageSprites == null || growthStageSprites.Length == 0) return null;

        float progress = daysToGrow <= 0 ? 1f : (float)daysGrown / daysToGrow;
        int index = Mathf.Clamp(Mathf.FloorToInt(progress * growthStageSprites.Length), 0, growthStageSprites.Length - 1);
        return growthStageSprites[index];
    }
}
