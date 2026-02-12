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
