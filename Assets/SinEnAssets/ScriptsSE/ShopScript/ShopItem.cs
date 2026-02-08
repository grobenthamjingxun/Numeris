using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Scriptable Objects/ShopItem")]
public class ShopItem : ScriptableObject
{
    public int shopItemId;
    public string shopItemName;
    public int price;
    public Sprite shopItemIcon;
    public ShopItemType itemType;
    public ItemTier itemTier;
    public enum ShopItemType
    {
        Consumable,
        PowerUp,
    }
    public enum ItemTier
    {
        Common,
        Rare,
        Epic
    }
    public InventoryItem correspondingInventoryItem;
}
