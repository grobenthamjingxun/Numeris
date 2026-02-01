using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private Player currentPlayer;

    private void Awake()
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

    public void SetPlayerData(Player player)
    {
        currentPlayer = player;
        if (ShopManager.instance != null)
        {
            ShopManager.instance.OnPlayerDataLoaded();
        }
    }

    public Player GetPlayerData()
    {
        return currentPlayer;
    }

    public int GetCoins()
    {
        //Return current coins from firebase
        if (currentPlayer != null)
        {
            return currentPlayer.coins;
        }
        return 100;
    }

    public void SetCoins(int amount)
    {
        if (currentPlayer != null)
        {
            currentPlayer.coins = amount;
        }
    }

    public void AddCoins(int amount)
    {
        if (currentPlayer != null)
        {
            currentPlayer.coins += amount;
            if (ShopManager.instance != null)
            {
                ShopManager.instance.UpdatePlayerCoinsDisplay(currentPlayer.coins);
            }
            // Update in Firebase
            FirebaseManager.Instance.UpdatePlayerField("coins", currentPlayer.coins,
                onSuccess: () => Debug.Log($"Added {amount} coins. Total: {currentPlayer.coins}"),
                onError: (error) => Debug.LogError("Failed to update coins: " + error)
            );
        }
    }

    public bool SpendCoins(int amount)
    {
        if (currentPlayer != null && currentPlayer.coins >= amount)
        {
            currentPlayer.coins -= amount;
            
            // Update in Firebase
            FirebaseManager.Instance.UpdatePlayerField("coins", currentPlayer.coins,
                onSuccess: () => Debug.Log($"Spent {amount} coins. Remaining: {currentPlayer.coins}"),
                onError: (error) => Debug.LogError("Failed to update coins: " + error)
            );
            
            return true;
        }
        return false;
    }

    public int GetHealth()
    {
        if (currentPlayer != null)
        {
            return currentPlayer.currentHealth;
        }
        return 100;
    }

    public void SetHealth(int health)
    {
        if (currentPlayer != null)
        {
            currentPlayer.currentHealth = health;
            
            // Update in Firebase
            FirebaseManager.Instance.UpdatePlayerField("currentHealth", currentPlayer.currentHealth,
                onSuccess: () => Debug.Log($"Health updated to: {currentPlayer.currentHealth}"),
                onError: (error) => Debug.LogError("Failed to update health: " + error)
            );
        }
    }
}