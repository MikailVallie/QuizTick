using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    private string scoresFilePath;
    private ScoreData scoreData = new ScoreData();

    // UI References - FIXED VARIABLE NAMES (no spaces)
    public LeaderboardEntryUI firstPlaceUI;
    public LeaderboardEntryUI secondPlaceUI;
    public LeaderboardEntryUI thirdPlaceUI;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContent;
    public TMP_Dropdown categoryDropdown;
    public TMP_Dropdown difficultyDropdown;

    void Start()
    {
        scoresFilePath = Path.Combine(Application.streamingAssetsPath, "player_scores.json");
        LoadScores();
        LoadCategories();

        if (scoreData.scores.Count == 0)
        {
            AddTestScores();
        }

        RefreshLeaderboard();
    }

    void LoadScores()
    {
        if (File.Exists(scoresFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(scoresFilePath);
                scoreData = JsonUtility.FromJson<ScoreData>(jsonData);
                Debug.Log($"Loaded {scoreData.scores.Count} scores from JSON");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading scores: {e.Message}");
                scoreData = new ScoreData();
            }
        }
        else
        {
            scoreData = new ScoreData();
            SaveScores();
            Debug.Log("Created new scores JSON file");
        }
    }

    void SaveScores()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(scoreData, true);
            File.WriteAllText(scoresFilePath, jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving scores: {e.Message}");
        }
    }

    void LoadCategories()
    {
        if (categoryDropdown == null)
        {
            Debug.LogError("CategoryDropdown is not assigned!");
            return;
        }

        categoryDropdown.ClearOptions();
        List<string> categories = new List<string> { "Python", "Java", "JavaScript", "C#", "SQL" };
        categoryDropdown.AddOptions(categories);

        if (difficultyDropdown == null)
        {
            Debug.LogError("DifficultyDropdown is not assigned!");
            return;
        }

        difficultyDropdown.ClearOptions();
        List<string> difficulties = new List<string> { "Easy", "Medium", "Hard" };
        difficultyDropdown.AddOptions(difficulties);

        categoryDropdown.onValueChanged.AddListener(delegate { RefreshLeaderboard(); });
        difficultyDropdown.onValueChanged.AddListener(delegate { RefreshLeaderboard(); });
    }

    public void RefreshLeaderboard()
    {
        if (categoryDropdown == null || difficultyDropdown == null)
        {
            Debug.LogError("Dropdowns are not assigned!");
            return;
        }

        string selectedCategory = categoryDropdown.options[categoryDropdown.value].text;
        string selectedDifficulty = difficultyDropdown.options[difficultyDropdown.value].text;
        LoadLeaderboard(selectedCategory, selectedDifficulty);
    }

    public void LoadLeaderboard(string category, string difficulty)
    {
        // Clear existing UI entries
        if (leaderboardContent == null)
        {
            Debug.LogError("LeaderboardContent is not assigned!");
            return;
        }

        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Get top scores for the selected category/difficulty
        List<PlayerScore> topScores = scoreData.GetTopScores(category, difficulty, 10);

        // Display the scores
        int rank = 1;
        foreach (var score in topScores)
        {
            if (rank == 1 && firstPlaceUI != null)
            {
                firstPlaceUI.SetEntry(rank, score.playerName, score.score);
            }
            else if (rank == 2 && secondPlaceUI != null)
            {
                secondPlaceUI.SetEntry(rank, score.playerName, score.score);
            }
            else if (rank == 3 && thirdPlaceUI != null)
            {
                thirdPlaceUI.SetEntry(rank, score.playerName, score.score);
            }
            else
            {
                if (leaderboardEntryPrefab != null)
                {
                    GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                    LeaderboardEntryUI entryUI = entry.GetComponent<LeaderboardEntryUI>();
                    if (entryUI != null)
                    {
                        entryUI.SetEntry(rank, score.playerName, score.score);
                    }
                }
            }
            rank++;
        }
    }

    public void SavePlayerScore(string playerName, string category, string difficulty, int scoreValue)
    {
        PlayerScore newScore = new PlayerScore(playerName, category, difficulty, scoreValue);
        scoreData.AddOrUpdateScore(newScore);
        SaveScores();
        RefreshLeaderboard();

        Debug.Log($"Score saved: {playerName} - {category} ({difficulty}): {scoreValue}");
    }

    // Test method (remove this in production)
    public void AddTestScores()
    {
        string[] testPlayers = { "MIKA", "SPADDQAH", "AVELA", "AVIWE", "PLAYER5" };
        string[] categories = { "Python", "Java", "JavaScript", "C#", "SQL" };
        string[] difficulties = { "Easy", "Medium", "Hard" };

        foreach (string player in testPlayers)
        {
            foreach (string category in categories)
            {
                foreach (string difficulty in difficulties)
                {
                    int score = Random.Range(100, 1000);
                    SavePlayerScore(player, category, difficulty, score);
                }
            }
        }
    }
    // Add these classes to your LeaderboardManager.cs file
    [System.Serializable]
    public class PlayerScore
    {
        public string playerName;
        public string category;
        public string difficulty;
        public int score;

        public PlayerScore(string name, string cat, string diff, int scr)
        {
            playerName = name;
            category = cat;
            difficulty = diff;
            score = scr;
        }
    }

    [System.Serializable]
    public class ScoreData
    {
        public List<PlayerScore> scores = new List<PlayerScore>();

        public List<PlayerScore> GetTopScores(string category, string difficulty, int count)
        {
            var filtered = scores.FindAll(s => s.category == category && s.difficulty == difficulty);
            filtered.Sort((a, b) => b.score.CompareTo(a.score));
            return filtered.GetRange(0, Mathf.Min(count, filtered.Count));
        }

        public void AddOrUpdateScore(PlayerScore newScore)
        {
            var existing = scores.FindIndex(s =>
                s.playerName == newScore.playerName &&
                s.category == newScore.category &&
                s.difficulty == newScore.difficulty);

            if (existing >= 0)
            {
                if (newScore.score > scores[existing].score)
                    scores[existing].score = newScore.score;
            }
            else
            {
                scores.Add(newScore);
            }
        }
    }
    
}
