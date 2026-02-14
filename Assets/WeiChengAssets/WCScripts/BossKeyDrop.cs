/*
* Author: Cheang Wei Cheng
* Date: 12/02/2026
* Description: This script is attached to the enemy that drops the boss key upon defeat.
* It listens for the enemy's death event and spawns the boss key prefab at the enemy's position when the enemy is defeated.
* This script is separate from the LootBag script since that script only accepts scriptable objects for loot drops,
* whereas the boss key is a prefab, and thus requires a different handling approach.
*/

using UnityEngine;

public class BossKeyDrop : MonoBehaviour
{
    public GameObject bossKeyPrefab;
    public EnemyBehaviour enemyBehaviour;
    void Start()
    {
        if (enemyBehaviour == null)
        {
            enemyBehaviour = GetComponent<EnemyBehaviour>();
        }
        if (bossKeyPrefab == null)
        {
            Debug.LogError("BossKeyDrop: bossKeyPrefab is not assigned!");
        }
    }
    
    /// <summary>
    /// This method is called when the enemy dies.
    /// It checks if the bossKeyPrefab is assigned, and if so, it instantiates the key at the enemy's position with a slight upward offset to prevent it from spawning inside the ground.
    /// </summary>
    public void DropKey()
    {
        if (bossKeyPrefab == null)
        {
            Debug.LogError("BossKeyDrop: Cannot drop key - bossKeyPrefab is null!");
            return;
        }
        
        // Instantiate the key at the enemy's position with default rotation
        Vector3 dropPosition = transform.position + Vector3.up * 1f; // Slightly above ground
        Instantiate(bossKeyPrefab, dropPosition, Quaternion.identity);
        Debug.Log($"Boss key dropped at {dropPosition}");
    }
}
