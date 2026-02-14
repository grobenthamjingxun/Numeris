/*
* Author: Cheang Wei Cheng
* Date: 08/02/2026
* Description: This script is attached to the portal that appears after the player kills a certain number of enemies.
* It keeps track of the number of enemies killed and activates the portal when the threshold is reached.
*/

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

    /// <summary>
    /// This method is called by the EnemyBehaviour script when an enemy is killed.
    /// It increments the kill count and updates the UI.
    /// If the kill count reaches the threshold, it activates the portal.
    /// </summary>
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

    /// <summary>
    /// This method is called when the player enters the portal.
    /// It clears the kill count UI text for when the player returns to the main lobby after completing the level, since the kill count is only relevant for the current level and should not carry over to the hub or future levels.
    /// </summary>
    public void OnPortalEnter()
    {
        if (killCountUI != null)
        {
            killCountUI.text = "";
        }
    }
}