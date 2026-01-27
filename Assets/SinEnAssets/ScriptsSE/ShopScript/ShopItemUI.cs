using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;
    
    [Header("Visual Settings")]
    [SerializeField] private Color affordableColor = Color.white;
    [SerializeField] private Color unaffordableColor = Color.gray;
    [SerializeField] private float buttonPulseScale = 1.1f;
    [SerializeField] private float pulseDuration = 0.2f;
    
    private ShopItem shopItem;
    private ShopManager shopManager;
    
    public void Initialize(ShopItem item, ShopManager manager)
    {
        shopItem = item;
        shopManager = manager;
        
        // Null check for each UI element
        if (itemIcon != null && item.shopIcon != null)
            itemIcon.sprite = item.shopIcon;
        else if (itemIcon == null)
            Debug.LogError("ItemIcon is not assigned in ShopItemUI prefab!");
            
        if (itemNameText != null)
            itemNameText.text = item.shopItemName;
        else
            Debug.LogError("ItemNameText is not assigned in ShopItemUI prefab!");
            
        if (priceText != null)
            priceText.text = $"{item.cost} Coins";
        else
            Debug.LogError("PriceText is not assigned in ShopItemUI prefab!");
        
        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        else
            Debug.LogError("BuyButton is not assigned in ShopItemUI prefab!");
        
        UpdateButtonState();
    }
    
    public void UpdateButtonState()
    {
        if (CoinManager.Instance == null || buyButton == null || shopItem == null)
        {
            Debug.LogError("CoinManager, BuyButton, or ShopItem is null!");
            return;
        }
        
        bool canAfford = CoinManager.Instance.CanAfford(shopItem.cost);
        
        buyButton.interactable = canAfford;
        
        Color targetColor = canAfford ? affordableColor : unaffordableColor;
        
        if (itemIcon != null)
            itemIcon.color = targetColor;
            
        if (priceText != null)
            priceText.color = targetColor;
    }
    
    void OnBuyButtonClicked()
    {
        shopManager.PurchaseItem(shopItem, this);
    }
    
    public void PlayPurchaseAnimation()
    {
        if (buyButton != null)
            StartCoroutine(PulseButton());
    }
    
    IEnumerator PulseButton()
    {
        if (buyButton == null) yield break;
        
        Vector3 originalScale = buyButton.transform.localScale;
        Vector3 targetScale = originalScale * buttonPulseScale;
        
        float elapsed = 0f;
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            buyButton.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            buyButton.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        buyButton.transform.localScale = originalScale;
    }
}