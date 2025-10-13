using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public string category;
    public string difficulty;
    public int score;
    public string dateAchieved;

    public PlayerScore() { }

    public PlayerScore(string name, string cat, string diff, int scr)
    {
        playerName = name;
        category = cat;
        difficulty = diff;
        score = scr;
        dateAchieved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class ScoreData
{
    public List<PlayerScore> scores = new List<PlayerScore>();

    // Method to add or update a score
    public void AddOrUpdateScore(PlayerScore newScore)
    {
        // Check if player already has a score for this category/difficulty
        PlayerScore existingScore = scores.Find(score =>
            score.playerName == newScore.playerName &&
            score.category == newScore.category &&
            score.difficulty == newScore.difficulty);

        if (existingScore != null)
        {
            // Update if new score is higher
            if (newScore.score > existingScore.score)
            {
                existingScore.score = newScore.score;
                existingScore.dateAchieved = newScore.dateAchieved;
            }
        }
        else
        {
            // Add new score
            scores.Add(newScore);
        }
    }

    // Method to get top scores for a category/difficulty
    public List<PlayerScore> GetTopScores(string category, string difficulty, int maxCount = 10)
    {
        List<PlayerScore> filteredScores = scores.FindAll(score =>
            score.category == category && score.difficulty == difficulty);

        // Sort by score (highest first)
        filteredScores.Sort((a, b) => b.score.CompareTo(a.score));

        // Return top scores
        return filteredScores.GetRange(0, Math.Min(maxCount, filteredScores.Count));
    }
}
