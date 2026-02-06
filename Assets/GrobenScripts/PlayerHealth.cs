using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 100;
    public TMP_Text healthText;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Staff;
    [SerializeField] private SceneChanger sceneChanger;

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
            sceneChanger.ChangeScene();
            currentHealth = 100; // Reset health
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
