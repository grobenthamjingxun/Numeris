using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

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
        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player == null)
        {
            Debug.LogWarning("PlayerHealth not found!");
            return;
        }
        player.TryHeal(25); // or use a configurable healAmount
    }

    // ----------------------
    // 50:50
    // ----------------------
    public void UseFiftyFifty()
    {
        FiftyFiftyPowerUp fifty = Object.FindFirstObjectByType<FiftyFiftyPowerUp>();
        if (fifty == null)
        {
            Debug.LogWarning("[PowerUpManager] FiftyFiftyPowerUp not found in scene.");
            return;
        }
        fifty.Activate();
    }

    // ----------------------
    // SWITCH QUESTION
    // ----------------------
    public void UseSwitchQuestion()
    {
        SwitchQuestionPowerUp sw = Object.FindFirstObjectByType<SwitchQuestionPowerUp>();
        if (sw == null)
        {
            Debug.LogWarning("[PowerUpManager] SwitchQuestionPowerUp not found in scene.");
            return;
        }
        sw.Activate();
    }

    public int GetPowerUpCount(PowerUpType type)
    {
        if (InvenManager.instance == null) return 0;

        int count = 0;
        foreach (var item in InvenManager.instance.invenItemList)
        {
            if (item.powerUpType == type)
            {
                count += item.invenQuantity;
            }
        }
        return count;
    }
}