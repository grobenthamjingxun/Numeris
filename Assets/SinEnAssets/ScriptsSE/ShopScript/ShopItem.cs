using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Scriptable Objects/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string shopId;
    public string shopItemName;
    public Sprite shopIcon;
    public int cost;
    public ShopItemType shopItemType;
    public int effectValue;
}

public enum ShopItemType
{
    Food,
    HealthBoost,
    SlowTimeBoost,
    Shield
}