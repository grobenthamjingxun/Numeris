using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 100;
    public TMP_Text healthText;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Staff;

    void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateUI();
        if (currentHealth == 0)
        {
            Debug.Log("Player is dead!");
            Player.transform.position = Vector3.zero; // Respawn at origin
            currentHealth = 100; // Reset health
            Staff.transform.position = new Vector3(0, 1, 0.3f); // Reset staff position
            UpdateUI();
        }
    }

    public bool TryHeal(int amount)
    {
        currentHealth += amount;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }
}
