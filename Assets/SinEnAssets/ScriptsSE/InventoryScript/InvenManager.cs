using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class InvenManager : MonoBehaviour
{
    public static InvenManager instance;
    public List<InventoryItem> invenItemList = new List<InventoryItem>();

    public Transform invenItemContent;
    public GameObject invenItemPrefab;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(InventoryItem itemToAdd)
    {
        InventoryItem existingItem = invenItemList.Find(item => item.invenId == itemToAdd.invenId);
        if (existingItem != null)
        {
            existingItem.invenQuantity += itemToAdd.invenQuantity;
            Debug.Log("Increased quantity of: " + existingItem.invenItemName + " to " + existingItem.invenQuantity);
        }
        else
        {
            invenItemList.Add(itemToAdd);
            Debug.Log("Added new item: " + itemToAdd.invenItemName);
        }
    }

    public void DisplayInventory()
    {
        foreach (Transform child in invenItemContent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem invenItem in invenItemList)
        {
            GameObject itemObj = Instantiate(invenItemPrefab, invenItemContent);
            
            // Find and check InvenItemName
            Transform nameTransform = itemObj.transform.Find("InvenItemName");
            if (nameTransform == null)
            {
                Debug.LogError("InvenItemName not found in prefab!");
                continue;
            }
            TextMeshProUGUI invenItemName = nameTransform.GetComponent<TextMeshProUGUI>();
            if (invenItemName == null)
            {
                Debug.LogError("Text component not found on InvenItemName!");
                continue;
            }
            
            // Find and check InvenImage
            Transform imageTransform = itemObj.transform.Find("InvenImage");
            if (imageTransform == null)
            {
                Debug.LogError("InvenImage not found in prefab!");
                continue;
            }
            Image invenItemImage = imageTransform.GetComponent<Image>();
            if (invenItemImage == null)
            {
                Debug.LogError("Image component not found on InvenImage!");
                continue;
            }
            
            // Set values
            invenItemName.text = invenItem.invenItemName;
            if (invenItem.invenIcon != null)
            {
                invenItemImage.sprite = invenItem.invenIcon;
            }
            else
            {
                Debug.LogWarning($"Icon is null for item: {invenItem.invenItemName}");
            }
        }
    }
}
