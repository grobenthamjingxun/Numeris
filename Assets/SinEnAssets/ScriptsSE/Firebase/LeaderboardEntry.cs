using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text usernameText;
    public TMP_Text coinText;

    public void SetEntry(int rank, string username, int coins)
    {
        rankText.text = rank.ToString();
        usernameText.text = username;
        coinText.text = coins.ToString("N0");
    }
}
