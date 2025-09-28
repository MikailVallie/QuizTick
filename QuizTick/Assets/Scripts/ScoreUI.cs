using UnityEngine;
using TMPro;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class ScoreUI : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;
    public TMP_Dropdown difficultyDropdown;
    public TMP_Text topScoreText;

    private string dbPath;

    void Awake()
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");
    }

    void OnEnable()
    {
        UpdateTopScore();
    }


    public void OnDropdownChanged()
    {
        UpdateTopScore();
    }


    void UpdateTopScore()
    {
        if (string.IsNullOrEmpty(GameSession.LoggedInUsername))
        {
            topScoreText.text = "Not logged in";
            return;
        }

        if (!File.Exists(dbPath))
        {
            topScoreText.text = "DB missing";
            return;
        }

        string selectedLanguage = languageDropdown.options[languageDropdown.value].text.Trim();
        string selectedLevel = difficultyDropdown.options[difficultyDropdown.value].text.Trim();
        string username = GameSession.LoggedInUsername.Trim();

        using (var db = new SQLiteConnection(dbPath))
        {
            // Debug log to see what values we're querying
            Debug.Log($"Looking for score: Username={username}, Language={selectedLanguage}, Level={selectedLevel}");

            var topScore = db.Table<Score>()
                .Where(s =>
                    s.Username.ToLower() == username.ToLower() &&
                    s.Language.ToLower() == selectedLanguage.ToLower() &&
                    s.Level.ToLower() == selectedLevel.ToLower())
                .OrderByDescending(s => s.Value)
                .FirstOrDefault();

            if (topScore != null)
            {
                Debug.Log($"Found score: {topScore.Value}");
                topScoreText.text = $"Top Score: {topScore.Value}";
            }
            else
            {
                Debug.Log("No matching score found.");
                topScoreText.text = "     0    ";
            }
        }
    }
}
