using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public int invenId;
    public string invenItemName;
    public int invenQuantity;
    public Sprite invenIcon;
    public ItemTier itemTier;
    public int dropChance;
    public GameObject lootPrefab3D;
}

public enum ItemTier
{
    Common,
    Rare,
    Epic
}