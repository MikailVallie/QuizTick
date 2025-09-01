using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyManager : MonoBehaviour
{
    public void SelectDifficulty(string difficulty)
    {
        GameData.Instance.SelectedDifficulty = difficulty;
        Debug.Log("Difficulty selected: " + difficulty);
        SceneManager.LoadScene("Quiz");
    }
}

