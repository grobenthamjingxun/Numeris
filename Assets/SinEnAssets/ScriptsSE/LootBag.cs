using UnityEngine;
using System.Collections.Generic;

public class LootBag : MonoBehaviour
{
    [Header("Loot Settings")]
    public List<InventoryItem> lootList = new List<InventoryItem>();
    
    [Header("Drop Physics")]
    public float dropHeight = 1f;
    public float spreadRadius = 1f;

    public void DropLoot()
    {
        InventoryItem droppedItem = GetDroppedItem();
        if (droppedItem != null)
        {
            Vector3 dropPosition = transform.position + Vector3.up * dropHeight;
            InstantiateLoot(droppedItem, dropPosition);
        }
    }

    InventoryItem GetDroppedItem()
    {
        // Roll a random number for each item
        List<InventoryItem> possibleItems = new List<InventoryItem>();
        foreach (InventoryItem item in lootList)
        {
            int roll = Random.Range(0, 101); // 0-100
            
            if (roll <= item.dropChance)
            {
                possibleItems.Add(item);
                Debug.Log($"Item {item.invenItemName} passed drop check! (Roll: {roll}, Chance: {item.dropChance})");
            }
        }
        if (possibleItems.Count > 0)
        {
            // Pick one random item from the possible drops
            InventoryItem droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            Debug.Log($"Dropping: {droppedItem.invenItemName}");
            return droppedItem;
        }
        Debug.Log("No item dropped");
        return null;
    }

    void InstantiateLoot(InventoryItem item, Vector3 spawnPosition)
    {
        GameObject lootPrefab = item.lootPrefab3D;
        
        if (lootPrefab == null)
        {
            Debug.LogError($"No 3D loot prefab assigned for {item.invenItemName}!");
            return;
        }

        // Add random spread to drop position
        Vector3 randomOffset = new Vector3(
            Random.Range(-spreadRadius, spreadRadius),
            0f,
            Random.Range(-spreadRadius, spreadRadius)
        );
        
        Vector3 finalPosition = spawnPosition + randomOffset;
        
        // Instantiate the loot
        GameObject lootObject = Instantiate(lootPrefab, finalPosition, Quaternion.identity);
        
        // Setup the loot component
        LootItem lootComponent = lootObject.GetComponent<LootItem>();
        if (lootComponent == null)
        {
            lootComponent = lootObject.AddComponent<LootItem>();
        }
        lootComponent.InitializeLoot(item);
        
        // Apply physics if there's a Rigidbody
        Rigidbody rb = lootObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            //Add Rotation
            Vector3 randomTorque = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }
}