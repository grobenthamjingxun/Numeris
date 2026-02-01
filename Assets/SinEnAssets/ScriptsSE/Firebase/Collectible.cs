using UnityEngine;
using System;

[Serializable]
public class Collectible
{
    public string collectibleID;
    public string collectibleName;
    public string tier;
    public int quantity;
    public Collectible() { }
    public Collectible(string collectibleID, string collectibleName, string tier, int quantity)
    {
        this.collectibleID = collectibleID;
        this.collectibleName = collectibleName;
        this.tier = tier;
        this.quantity = quantity;
    }
}
