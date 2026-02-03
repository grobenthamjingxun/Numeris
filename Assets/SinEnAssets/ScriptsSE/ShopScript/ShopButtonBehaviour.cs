using UnityEngine;
using UnityEngine.UI;

public class ShopButtonBehaviour : MonoBehaviour
{
    public ShopItem shopItem;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Initialize(ShopItem item)
    {
        shopItem = item;
    }

    public void OnPurchaseButtonClicked()
    {
        Debug.Log($"Purchase button clicked for: {shopItem.shopItemName}");
        
        if (ShopManager.instance != null && shopItem != null)
        {
            ShopManager.instance.PurchaseItem(shopItem);
        }
        else
        {
            Debug.LogError("ShopManager instance or shopItem is null!");
        }
    }
}