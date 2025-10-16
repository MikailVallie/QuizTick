using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{

    public LeaderboardEntryUI firstPlaceUI;
    public LeaderboardEntryUI secondPlaceUI;
    public LeaderboardEntryUI thirdPlaceUI;
    public LeaderboardEntryUI fourthPlaceUI;  
    public LeaderboardEntryUI fifthPlaceUI;     
    public LeaderboardEntryUI sixthPlaceUI;   
    public Transform leaderboardContent;
    public TMP_Dropdown categoryDropdown;
    public TMP_Dropdown difficultyDropdown;
    public TextMeshProUGUI headingText;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Start()
    {
        LoadCategories();
        RefreshLeaderboard();
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

        
        if (headingText != null) 
        {
            headingText.text = selectedCategory.ToUpper();
        }

        LoadLeaderboard(selectedCategory, selectedDifficulty);
    }

    public void LoadLeaderboard(string category, string difficulty)
    {
        
        List<Score> topScores = ScoreManager.GetTopScores(category, difficulty, 6);

        
        for (int i = 0; i < topScores.Count; i++)
        {
            var score = topScores[i];
            int rank = i + 1;

            switch (rank)
            {
                case 1:
                    if (firstPlaceUI != null)
                        firstPlaceUI.SetEntry(rank, score.Username, score.Value);
                    break;
                case 2:
                    if (secondPlaceUI != null)
                        secondPlaceUI.SetEntry(rank, score.Username, score.Value);
                    break;
                case 3:
                    if (thirdPlaceUI != null)
                        thirdPlaceUI.SetEntry(rank, score.Username, score.Value);
                    break;
                case 4:
                    if (fourthPlaceUI != null)
                        fourthPlaceUI.SetEntry(rank, score.Username, score.Value);
                    break;
                case 5:
                    if (fifthPlaceUI != null)
                        fifthPlaceUI.SetEntry(rank, score.Username, score.Value);
                    break;
                case 6:
                    if (sixthPlaceUI != null)
                        sixthPlaceUI.SetEntry(rank, score.Username, score.Value);
                    break;
            }
        }

        for (int i = topScores.Count; i < 6; i++)
        {
            int rank = i + 1;
            switch (rank)
            {
                case 4:
                    if (fourthPlaceUI != null) fourthPlaceUI.SetEntry(rank, "---", 0);
                    break;
                case 5:
                    if (fifthPlaceUI != null) fifthPlaceUI.SetEntry(rank, "---", 0);
                    break;
                case 6:
                    if (sixthPlaceUI != null) sixthPlaceUI.SetEntry(rank, "---", 0);
                    break;
            }
        }

        Debug.Log($"Loaded {topScores.Count} scores for {category} - {difficulty}");
    }

    public void SavePlayerScore(string playerName, string category, string difficulty, int scoreValue)
    {
        ScoreManager.SaveScore(playerName, category, difficulty, scoreValue);
        RefreshLeaderboard();
        Debug.Log($"Score saved to database: {playerName} - {category} ({difficulty}): {scoreValue}");
    }

}