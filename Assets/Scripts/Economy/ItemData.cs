using UnityEngine;

// Stores the basic data for one item.
[CreateAssetMenu(fileName = "NewItem", menuName = "AlderBrook/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea(2, 4)] public string description;
    public Sprite icon;
    public int buyPrice = 20;
    public int sellPrice = 10;
}
