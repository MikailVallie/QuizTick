using UnityEngine;
using TMPro;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class ScoreUI : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown languageDropdown;
    public TMP_Dropdown difficultyDropdown;

    [Header("Score Display")]
    public TMP_Text topScoreText;

    private string dbPath;

    void Awake()
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");
    }

    void OnEnable()
    {
        UpdateTopScore();

        languageDropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });
        difficultyDropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });
    }

    void OnDisable()
    {
        languageDropdown.onValueChanged.RemoveAllListeners();
        difficultyDropdown.onValueChanged.RemoveAllListeners();
    }

    public void OnDropdownChanged()
{
    Debug.Log("Dropdown changed! Updating score...");
    UpdateTopScore();
}
void UpdateTopScore()
{
    if (string.IsNullOrEmpty(GameSession.LoggedInUsername))
    {
        topScoreText.text = "0";
        return;
    }

    if (!File.Exists(dbPath))
    {
        topScoreText.text = "0";
        return;
    }

    string selectedLanguage = languageDropdown.options[languageDropdown.value].text.Trim();
    string selectedLevel = difficultyDropdown.options[difficultyDropdown.value].text.Trim();
    string username = GameSession.LoggedInUsername.Trim();


    using (var db = new SQLiteConnection(dbPath))
    {
        var topScore = db.Table<Score>()
            .Where(s =>
                s.Username.ToLower() == username.ToLower() &&
                s.Language.ToLower() == selectedLanguage.ToLower() &&
                s.Level.ToLower() == selectedLevel.ToLower())
            .OrderByDescending(s => s.Value)
            .FirstOrDefault();

        if (topScore != null)
        {
          
            topScoreText.text = topScore.Value.ToString();
        }
        else
        {
           
            topScoreText.text = "0";
        }
    }
}

}

