using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform leaderboardContainer;
    public GameObject leaderboardEntryPrefab;

    [Header("Settings")]
    public int displayCount = 10;

    /// <summary>
    /// Call this whenever the leaderboard panel is opened or needs a refresh.
    /// </summary>
    public void RefreshLeaderboard()
    {
        FirebaseManager.Instance.FetchAllPlayers(OnPlayersLoadedDictionary, OnError);
    }

    // Fix: Accept Dictionary<string, Player> as required by FetchAllPlayers, then convert to List<Player>
    private void OnPlayersLoadedDictionary(Dictionary<string, Player> playerDict)
    {
        List<Player> players = playerDict?.Values.ToList() ?? new List<Player>();
        OnPlayersLoaded(players);
    }

    private void OnPlayersLoaded(List<Player> players)
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        // Sort by coins descending and take the top N
        List<Player> sorted = players
            .OrderByDescending(p => p.coins)
            .Take(displayCount)
            .ToList();

        // Spawn and populate each entry
        for (int i = 0; i < sorted.Count; i++)
        {
            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            LeaderboardEntry leaderboardEntry = entry.GetComponent<LeaderboardEntry>();
            leaderboardEntry.SetEntry(i + 1, sorted[i].username, sorted[i].coins);
        }
    }

    private void OnError(string error)
    {
        Debug.LogError("Leaderboard fetch failed: " + error);
    }
}