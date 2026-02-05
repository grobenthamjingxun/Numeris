using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("Health Power-Up")]
    public int healthPotions = 3;
    public int healAmount = 25;

    [Header("50:50 Power-Up")]
    public int fiftyFiftyCount = 3;

    [Header("Switch Question Power-Up")]
    public int switchQuestionCount = 2;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    // ----------------------
    // HEALTH
    // ----------------------
    public void UseHealth()
    {
        if (healthPotions <= 0) return;

        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player == null) return;

        player.TryHeal(healAmount); // unlimited health system
        healthPotions--;
    }

    public void AddHealthPotion(int amount = 1)
    {
        healthPotions += amount;
    }

    // ----------------------
    // 50:50
    // ----------------------
    public void UseFiftyFifty()
    {
        if (fiftyFiftyCount <= 0) return;

        FiftyFiftyPowerUp fifty = Object.FindFirstObjectByType<FiftyFiftyPowerUp>();
        if (fifty == null)
        {
            Debug.LogWarning("[PowerUpManager] FiftyFiftyPowerUp not found in scene.");
            return;
        }

        fifty.Activate();
        fiftyFiftyCount--;
    }

    public void AddFiftyFifty(int amount = 1)
    {
        fiftyFiftyCount += amount;
    }

    // ----------------------
    // SWITCH QUESTION (skip + enemy dies)
    // ----------------------
    public void UseSwitchQuestion()
    {
        if (switchQuestionCount <= 0) return;

        SwitchQuestionPowerUp sw = Object.FindFirstObjectByType<SwitchQuestionPowerUp>();
        if (sw == null)
        {
            Debug.LogWarning("[PowerUpManager] SwitchQuestionPowerUp not found in scene.");
            return;
        }

        sw.Activate();
        switchQuestionCount--;
    }

    public void AddSwitchQuestion(int amount = 1)
    {
        switchQuestionCount += amount;
    }
}
