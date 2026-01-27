using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }
    
    private List<ShopItem> ownedItems = new List<ShopItem>();
    
    [Header("Player Stats")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public bool hasShield = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddItem(ShopItem item)
    {
        ownedItems.Add(item);
        ApplyItemEffect(item);
        Debug.Log($"Added {item.shopItemName} to inventory");
    }
    
    void ApplyItemEffect(ShopItem item)
    {
        switch (item.shopItemType)
        {
            case ShopItemType.Food:
                currentHealth = Mathf.Min(currentHealth + item.effectValue, maxHealth);
                Debug.Log($"Healed {item.effectValue} HP. Current health: {currentHealth}");
                break;
                
            case ShopItemType.HealthBoost:
                maxHealth += item.effectValue;
                currentHealth += item.effectValue;
                Debug.Log($"Max health increased by {item.effectValue}");
                break;
                
            case ShopItemType.SlowTimeBoost:
                // You can implement slow time effect here
                // Example: Time.timeScale = 0.5f;
                Debug.Log($"Slow Time activated with value: {item.effectValue}");
                break;
                
            case ShopItemType.Shield:
                hasShield = true;
                Debug.Log("Shield activated!");
                break;
        }
    }
    
    public List<ShopItem> GetOwnedItems()
    {
        return new List<ShopItem>(ownedItems);
    }
}