using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string username;
    public string email;
    public bool isLoggedIn;
    public int currentLevel;
    public int currentHealth;
    public int coins;
    public List<Inventory> inventoryItems = new List<Inventory>();

    //Default Constructor
    public Player()
    {
        this.inventoryItems = new List<Inventory>();
        this.isLoggedIn = false;
        this.currentLevel = 0;;
        this.currentHealth = 100;
        this.coins = 100;
    }

    // Constructor for creating a new player
    public Player(string username, string email)
    {
        this.username = username;
        this.email = email;
        this.isLoggedIn = false;
        this.currentLevel = 0;
        this.currentHealth = 100;
        this.coins = 100;
        this.inventoryItems = new List<Inventory>();
    }
}
