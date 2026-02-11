using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private LootBag lootBag;
    public DissolveController dissolveController;
    public int maxHealth = 1;
    public int currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
        lootBag = GetComponent<LootBag>();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Portal.Instance?.OnEnemyKilled();
        GetCoinsOnDeath();
        if (dissolveController != null)
        {
            if (lootBag != null)
            {
                lootBag.DropLoot();
            }
            StartCoroutine(DieWithDissolve());
        }
        else
        {
            Destroy(gameObject);
        }
        GameObject[] correctOrbs = GameObject.FindGameObjectsWithTag("CorrectOrb");
        foreach (GameObject correctOrb in correctOrbs)
        {
            if (correctOrb != null)
            {
                correctOrb.SetActive(false);
            }
        }
    }

    IEnumerator DieWithDissolve()
    {
        // Disable collider so enemy can't be damaged while dissolving
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        yield return StartCoroutine(dissolveController.DissolveEffect());
        Destroy(gameObject);
    }

    void GetCoinsOnDeath()
    {
        if (PlayerManager.Instance != null)
        {
            int coinsToGive = Random.Range(5, 16); // Random coins between 5 and 15
            PlayerManager.Instance.AddCoins(coinsToGive);
            Debug.Log($"Player received {coinsToGive} coins from {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("PlayerManager instance not found. Cannot add coins.");
        }
    }
}