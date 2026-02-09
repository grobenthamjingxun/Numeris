using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    public static Portal Instance { get; private set; }
    
    [SerializeField] private int enemyKillThreshold = 2;
    private int enemiesKilled = 0;
    [SerializeField] private TMP_Text killCountUI;
    
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
        if (killCountUI == null)
        {
            killCountUI = GameObject.Find("KillCount").GetComponent<TMP_Text>();
        }
        this.gameObject.SetActive(false);
        UpdateKillCountUI();
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        UpdateKillCountUI();
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
        UpdateKillCountUI();
    }

    private void UpdateKillCountUI()
    {
        if (killCountUI != null)
        {
            killCountUI.text = $"{enemiesKilled}/{enemyKillThreshold}";
        }
    }

    public void OnPortalEnter()
    {
        if (killCountUI != null)
        {
            killCountUI.text = "";
        }
    }
}