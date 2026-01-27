using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    
    [Header("UI References")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private FeedbackManager feedbackManager;
    
    // [Header("Audio")]
    // [SerializeField] private AudioSource audioSource;
    // [SerializeField] private AudioClip purchaseSuccessSound;
    // [SerializeField] private AudioClip purchaseFailSound;
    
    private List<ShopItemUI> itemUIList = new List<ShopItemUI>();
    
    void Start()
    {
        PopulateShop();
        CoinManager.Instance.OnCoinsChanged.AddListener(OnCoinsChanged);
    }
    
    void PopulateShop()
    {
        foreach (ShopItem item in shopItems)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, itemContainer);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(item, this);
                itemUIList.Add(itemUI);
            }
        }
    }
    
    public void PurchaseItem(ShopItem item, ShopItemUI itemUI)
    {
        if (CoinManager.Instance.SpendCoins(item.cost))
        {
            PlayerInventory.Instance.AddItem(item);
            itemUI.PlayPurchaseAnimation();
            feedbackManager.ShowPurchaseSuccess(item.shopItemName);
            
            // if (audioSource != null && purchaseSuccessSound != null)
            // {
            //     audioSource.PlayOneShot(purchaseSuccessSound);
            // }
        }
        else
        {
            feedbackManager.ShowPurchaseFailed();
            
            // if (audioSource != null && purchaseFailSound != null)
            // {
            //     audioSource.PlayOneShot(purchaseFailSound);
            // }
        }
    }
    
    void OnCoinsChanged(int newAmount)
    {
        foreach (ShopItemUI itemUI in itemUIList)
        {
            itemUI.UpdateButtonState();
        }
    }
    
    void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged.RemoveListener(OnCoinsChanged);
        }
    }
}