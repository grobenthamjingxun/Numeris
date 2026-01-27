using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public int invenId;
    public string invenItemName;
    public int invenQuantity;
    public Sprite invenIcon;
}
