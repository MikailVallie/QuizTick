using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text playerNameText;
    public TMP_Text scoreText;

    public void SetEntry(int rank, string playerName, int score)
    {
        if (rankText != null) rankText.text = GetRankSuffix(rank);
        if (playerNameText != null) playerNameText.text = playerName;
        if (scoreText != null) scoreText.text = score.ToString();

        // Highlight top 3
        if (rankText != null)
        {
            if (rank == 1) rankText.color = Color.yellow;
            else if (rank == 2) rankText.color = Color.gray;
            else if (rank == 3) rankText.color = new Color(0.8f, 0.5f, 0.2f);
        }
    }

    private string GetRankSuffix(int rank)
    {
        if (rank == 1) return "1ST";
        if (rank == 2) return "2ND";
        if (rank == 3) return "3RD";
        return rank + "TH";
    }
}