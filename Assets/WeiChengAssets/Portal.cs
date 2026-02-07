using UnityEngine;

public class Portal : MonoBehaviour
{
    public static Portal Instance { get; private set; }
    
    [SerializeField] private int enemyKillThreshold = 2;
    private int enemiesKilled = 0;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Portal: Enemy killed! Count: {enemiesKilled}/{enemyKillThreshold}");
        
        if (enemiesKilled >= enemyKillThreshold)
        {
            this.gameObject.SetActive(true);
            Debug.Log("Portal activated!");
        }
    }
    
    public void ResetKillCount()
    {
        enemiesKilled = 0;
        this.gameObject.SetActive(false);
    }
}