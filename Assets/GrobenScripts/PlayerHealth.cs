using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 100;
    public TMP_Text healthText;

    void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateUI();
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
