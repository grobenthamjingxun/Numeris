using UnityEngine;

public class SwitchQuestionPowerUp : MonoBehaviour
{
    [Header("References (can be left empty, auto-finds)")]
    public TargetSelector targetSelector;
    public MathQuestionGenerator mathQuestionGenerator;

    [Header("Kill Settings")]
    public int killDamage = 999999;

    private GameObject currentTargetEnemy;

    private void Awake()
    {
        if (targetSelector == null)
            targetSelector = Object.FindFirstObjectByType<TargetSelector>();

        if (mathQuestionGenerator == null)
            mathQuestionGenerator = Object.FindFirstObjectByType<MathQuestionGenerator>();
    }

    private void OnEnable()
    {
        if (targetSelector != null)
            targetSelector.OnTargetLocked += OnTargetLocked;
    }

    private void OnDisable()
    {
        if (targetSelector != null)
            targetSelector.OnTargetLocked -= OnTargetLocked;
    }

    private void OnTargetLocked(GameObject target)
    {
        currentTargetEnemy = target;
    }

    // Call this from PowerUpManager
    public void Activate()
    {
        // 1) Clear question + orbs immediately
        if (mathQuestionGenerator != null)
        {
            mathQuestionGenerator.ClearCurrentQuestion();
        }

        // 2) Kill the currently locked enemy
        if (currentTargetEnemy == null)
        {
            Debug.LogWarning("[SwitchQuestion] No current target enemy stored. Make sure TargetSelector exists and locks targets.");
            return;
        }

        EnemyBehaviour enemy = currentTargetEnemy.GetComponent<EnemyBehaviour>();
        if (enemy == null)
        {
            Debug.LogWarning("[SwitchQuestion] Current target has no EnemyBehaviour. Cannot kill.");
            return;
        }

        enemy.TakeDamage(killDamage);
        Debug.Log("[SwitchQuestion] Activated: question skipped + enemy killed.");
    }
}
