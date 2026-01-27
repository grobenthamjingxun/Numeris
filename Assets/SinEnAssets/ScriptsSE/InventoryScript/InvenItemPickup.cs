using UnityEngine;

public class InvenItemPickup : MonoBehaviour
{
    public InventoryItem invenItem;
    public void Pickup()
    {
        InvenManager.instance.AddItem(invenItem);
        Debug.Log("Picked up: " + invenItem.invenItemName);
        Destroy(gameObject);
    }
}
