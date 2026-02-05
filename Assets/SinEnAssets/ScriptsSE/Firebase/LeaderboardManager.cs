using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References - Assign Prefabs Only")]
    public GameObject leaderboardEntryPrefab;
    private Transform leaderboardContainer;

    [Header("Settings")]
    public int displayCount = 10;

    public void OpenLeaderboard()
    {
        Debug.Log("=== Opening Leaderboard ===");
        
        // Find UI references in the scene
        FindUIReferences();
        
        // Fetch and display players
        RefreshLeaderboard();
    }

    private void FindUIReferences()
    {
        Debug.Log("Finding leaderboard UI references...");
        
        // Find LeaderboardCanvas in the scene
        GameObject leaderboardCanvas = GameObject.Find("LeaderboardCanvas");
        if (leaderboardCanvas == null)
        {
            Debug.LogError("LeaderboardCanvas not found! Make sure it's active in the scene.");
            return;
        }

        // Try to find the container - adjust this path to match YOUR hierarchy
        // Common paths: "Scroll View/Viewport/Content" or "Content" or "LeaderboardContainer"
        
        // Method 1: Direct child search
        Transform content = leaderboardCanvas.transform.Find("LeaderboardContent");
        if (content != null)
        {
            leaderboardContainer = content;
            Debug.Log("Found leaderboard container: Content");
            return;
        }

        // Method 2: Search through ScrollView
        Transform scrollView = leaderboardCanvas.transform.Find("Leaderboard");
        if (scrollView != null)
        {
            Transform viewport = scrollView.Find("Viewport");
            if (viewport != null)
            {
                content = viewport.Find("LeaderboardContent");
                if (content != null)
                {
                    leaderboardContainer = content;
                    Debug.Log("Found leaderboard container: Leaderboard/Viewport/LeaderboardContent");
                    return;
                }
            }
        }

        // Method 3: Search by name
        Transform[] allChildren = leaderboardCanvas.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name == "LeaderboardContent")
            {
                leaderboardContainer = child;
                Debug.Log($"Found leaderboard container: {child.name}");
                return;
            }
        }

        Debug.LogError("Could not find leaderboard container! Check your LeaderboardCanvas hierarchy.");
    }

    public void RefreshLeaderboard()
    {
        Debug.Log("Refreshing leaderboard...");
        
        if (FirebaseManager.Instance == null)
        {
            Debug.LogError("FirebaseManager.Instance is null!");
            return;
        }

        FirebaseManager.Instance.FetchAllPlayers(OnPlayersLoadedDictionary, OnError);
    }

    private void OnPlayersLoadedDictionary(Dictionary<string, Player> playerDict)
    {
        List<Player> players = playerDict?.Values.ToList() ?? new List<Player>();
        Debug.Log($"Fetched {players.Count} players for leaderboard");
        OnPlayersLoaded(players);
    }

    private void OnPlayersLoaded(List<Player> players)
    {
        Debug.Log("=== OnPlayersLoaded called ===");
        
        if (leaderboardContainer == null)
        {
            Debug.LogError("leaderboardContainer is NULL! Call FindUIReferences() first.");
            return;
        }

        if (leaderboardEntryPrefab == null)
        {
            Debug.LogError("leaderboardEntryPrefab is NULL! Assign it in Inspector.");
            return;
        }

        // CRITICAL: Check if parent is persistent (DontDestroyOnLoad)
        if (IsPersistent(leaderboardContainer.gameObject))
        {
            Debug.LogError("leaderboardContainer is persistent! Cannot instantiate with persistent parent. Make sure LeaderboardCanvas is NOT DontDestroyOnLoad.");
            return;
        }

        Debug.Log($"Clearing existing entries from: {leaderboardContainer.name}");
        
        // Clear existing entries
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        // Sort by coins descending
        List<Player> sorted = players
            .OrderByDescending(p => p.coins)
            .Take(displayCount)
            .ToList();

        Debug.Log($"Displaying top {sorted.Count} players");

        // Spawn and populate each entry
        for (int i = 0; i < sorted.Count; i++)
        {
            // FIXED: Instantiate without parent first
            GameObject entry = Instantiate(leaderboardEntryPrefab);
            
            // Then set parent
            entry.transform.SetParent(leaderboardContainer, false);
            
            // Setup the entry
            LeaderboardEntry leaderboardEntry = entry.GetComponent<LeaderboardEntry>();
            if (leaderboardEntry != null)
            {
                leaderboardEntry.SetEntry(i + 1, sorted[i].username, sorted[i].coins);
                Debug.Log($"#{i + 1}: {sorted[i].username} - {sorted[i].coins} coins");
            }
            else
            {
                Debug.LogError("LeaderboardEntry component not found on prefab!");
            }
        }

        Debug.Log("Leaderboard displayed successfully");
    }

    private void OnError(string error)
    {
        Debug.LogError($"Leaderboard fetch failed: {error}");
    }

    // Helper method to check if GameObject is persistent
    private bool IsPersistent(GameObject obj)
    {
        return obj.scene.name == null || obj.hideFlags == HideFlags.DontSave;
    }
}