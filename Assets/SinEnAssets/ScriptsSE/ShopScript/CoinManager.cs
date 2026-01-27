using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    
    [SerializeField] 
    private int startingCoins = 100;
    private int currentCoins;
    
    public UnityEvent<int> OnCoinsChanged;
    
    public int CurrentCoins => currentCoins;

    public TMPro.TextMeshProUGUI coinText;
    
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
    
    void Start()
    {
        currentCoins = startingCoins;
        coinText.text = currentCoins.ToString();
        OnCoinsChanged?.Invoke(currentCoins);
    }
    
    public bool CanAfford(int cost)
    {
        return currentCoins >= cost;
    }
    
    public bool SpendCoins(int amount)
    {
        if (CanAfford(amount))
        {
            currentCoins -= amount;
            OnCoinsChanged?.Invoke(currentCoins);
            coinText.text = currentCoins.ToString();
            Debug.Log($"Spent {amount} coins. Remaining: {currentCoins}");
            return true;
        }
        Debug.Log("Not enough coins!");
        return false;
    }
    
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
        coinText.text = currentCoins.ToString();
        Debug.Log($"Added {amount} coins. Total: {currentCoins}");
    }
}