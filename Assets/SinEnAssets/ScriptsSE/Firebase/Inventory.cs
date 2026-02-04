using UnityEngine;
using System;

[Serializable]
public class Inventory
{
    public string itemID;
    public Collectible collectibleDetails;
    public Inventory() { }

    public Inventory(string itemID)
    {
        this.itemID = itemID;
        this.collectibleDetails = new Collectible();
    }
}
