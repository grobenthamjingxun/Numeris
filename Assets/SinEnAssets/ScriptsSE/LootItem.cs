using UnityEngine;

public class LootItem : MonoBehaviour
{
    private InventoryItem itemItem;
    public float pickupRadius = 1.5f;

    [Header("Visual Effects")]
    public bool rotateItem = true;
    public float rotationSpeed = 50f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void InitializeLoot(InventoryItem item)
    {
        itemItem = item;
        gameObject.name = $"Loot_{item.invenItemName}";
        
        Debug.Log($"Loot initialized: {item.invenItemName}");
    }

    private void Update()
    {
        if (rotateItem)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void PickupItem()
    {
        if (itemItem == null)
        {
            Debug.LogError("No item data!");
            return;
        }
        InvenManager.instance.AddItem(itemItem);
        Debug.Log($"Picked up: {itemItem.invenItemName}");
        Destroy(gameObject);
    }
}
