using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public string SelectedCategory;
    public string SelectedDifficulty;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

