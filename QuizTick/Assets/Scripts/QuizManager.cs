using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    private int score = 0;
    private int streakCount = 0;
    private bool isGameOver = false;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;

    [Header("Timer Settings")]
    public Image timerImage;
    public float timePerQuestion = 15f;
    private float timeRemaining;
    private bool isAnswered = false;

    private int hintCount = 3;
    private int freezeCount = 3;
    private bool addTimeUsed = false;
    private bool isTimeFrozen = false;

    public Button hintButton;
    public Button freezeTimeButton;
    public Button addTimeButton;

    [Header("Power-Up Counters UI")]
    public TextMeshProUGUI hintCounterText;
    public TextMeshProUGUI freezeCounterText;
    public TextMeshProUGUI addTimeCounterText;

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options;
        public int correctIndex;
        public bool isTrueFalse;
        public string funFact;
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] questions;
    }

    private List<Question> questions;
    private Question currentQuestion;
    private int currentIndex = 0;

    public string selectedCategory;
    public string selectedDifficulty;

    [Header("Multiple Choice UI")]
    public GameObject MultipleChoice;
    public TextMeshProUGUI MCQuestion;
    public Button Option1Btn, Option2Btn, Option3Btn, Option4Btn;

    [Header("True/False UI")]
    public GameObject TF;
    public TextMeshProUGUI TFQuestion;
    public Button TrueBtn, FalseBtn;

    [Header("Fun Fact UI")]
    public GameObject funFactPanel;
    public TextMeshProUGUI funFactTitleText;
    public TextMeshProUGUI funFactText;


    private void Start()
    {
        isGameOver = false;

        selectedCategory = GameData.Instance.SelectedCategory;
        selectedDifficulty = GameData.Instance.SelectedDifficulty;

        if (hintButton != null) hintButton.onClick.AddListener(UseHint);
        if (freezeTimeButton != null) freezeTimeButton.onClick.AddListener(UseFreezeTime);
        if (addTimeButton != null) addTimeButton.onClick.AddListener(UseAddTime);

        LoadQuestions();
        StartQuestion();
        UpdateScoreText();
        UpdatePowerUpCounters();
    }

    private void LoadQuestions()
    {
        string fileName = selectedCategory + "_" + selectedDifficulty + ".json";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            QuestionList qList = JsonUtility.FromJson<QuestionList>(json);
            questions = new List<Question>(qList.questions);

            // Shuffle
            for (int i = 0; i < questions.Count; i++)
            {
                Question temp = questions[i];
                int rand = Random.Range(i, questions.Count);
                questions[i] = questions[rand];
                questions[rand] = temp;
            }

            int maxQuestions = Mathf.Min(20, questions.Count);
            questions = questions.GetRange(0, maxQuestions);

            Debug.Log($"Loaded {questions.Count} questions from {fileName}");
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
            questions = new List<Question>();
        }
    }
    private void Update()
    {
        if (isGameOver) return;

        if (!isAnswered && questions != null && questions.Count > 0 && !isTimeFrozen)
        {
            timeRemaining -= Time.deltaTime;

            if (timerImage != null)
                timerImage.fillAmount = timeRemaining / timePerQuestion;

            if (timeRemaining <= 0f)
                TimeOut();
        }
    }

    private void ResetTimer()
    {
        timeRemaining = timePerQuestion;
        isAnswered = false;
        if (timerImage != null)
            timerImage.fillAmount = 1f;
    }

    private void TimeOut()
    {
        if (isGameOver) return;

        isAnswered = true;
        streakCount = 0;

        HighlightCorrectAnswer();
        Invoke(nameof(NextQuestion), 2f);
    }

    private void StartQuestion()
    {
        if (isGameOver) return;

        if (currentIndex >= questions.Count)
        {
            QuizFinished();
            return;
        }

        currentQuestion = questions[currentIndex];
        ResetTimer();
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        ResetButtonColors();
        SetPowerUpButtonsActive(true);

        if (!currentQuestion.isTrueFalse)
        {
            MultipleChoice.SetActive(true);
            TF.SetActive(false);

            MCQuestion.text = currentQuestion.questionText;
            Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(i < currentQuestion.options.Length);
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.options[i];
                int index = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
        }
        else
        {
            MultipleChoice.SetActive(false);
            TF.SetActive(true);

            TFQuestion.text = currentQuestion.questionText;

            TrueBtn.onClick.RemoveAllListeners();
            FalseBtn.onClick.RemoveAllListeners();

            TrueBtn.onClick.AddListener(() => OnAnswerSelected(0));
            FalseBtn.onClick.AddListener(() => OnAnswerSelected(1));
        }

        if (funFactPanel != null)
            funFactPanel.SetActive(false);
    }

    private void ResetButtonColors()
    {
        Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };
        foreach (Button btn in buttons)
        {
            btn.image.color = Color.white;
            btn.gameObject.SetActive(true);
        }
        TrueBtn.image.color = Color.white;
        FalseBtn.image.color = Color.white;
    }
    private void OnAnswerSelected(int selectedIndex)
    {
        if (isGameOver || isAnswered) return;
        isAnswered = true;

        if (selectedIndex == currentQuestion.correctIndex)
        {
            score += 1;
            streakCount++;
            UpdateScoreText();

            if (streakCount == 3)
            {
                score += 5;
                Debug.Log("Streak Bonus! +5 points");
                UpdateScoreText();
            }

            SoundManager.Instance.PlaySound("CorrectAnswer");
            HighlightAnswer(selectedIndex, true);
        }
        else
        {
            streakCount = 0;
            SoundManager.Instance.PlaySound("IncorrectAnswer");
            HighlightAnswer(selectedIndex, false);
        }

        ShowFunFactIfAvailable();
    }

    private void HighlightAnswer(int selectedIndex, bool correct)
    {
        if (currentQuestion.isTrueFalse)
        {
            if (correct)
            {
                if (selectedIndex == 0) TrueBtn.image.color = Color.green;
                else FalseBtn.image.color = Color.green;
            }
            else
            {
                if (selectedIndex == 0) TrueBtn.image.color = Color.red;
                else FalseBtn.image.color = Color.red;
                if (currentQuestion.correctIndex == 0) TrueBtn.image.color = Color.green;
                else FalseBtn.image.color = Color.green;
            }
        }
        else
        {
            Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };
            if (correct)
                buttons[selectedIndex].image.color = Color.green;
            else
            {
                buttons[selectedIndex].image.color = Color.red;
                buttons[currentQuestion.correctIndex].image.color = Color.green;
            }
        }
    }

    private void HighlightCorrectAnswer()
    {
        if (currentQuestion.isTrueFalse)
        {
            if (currentQuestion.correctIndex == 0) TrueBtn.image.color = Color.green;
            else FalseBtn.image.color = Color.green;
        }
        else
        {
            Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };
            buttons[currentQuestion.correctIndex].image.color = Color.green;
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    private void UpdatePowerUpCounters()
    {
        if (hintCounterText != null)
            hintCounterText.text = $"Hints: {hintCount}/3";
        if (freezeCounterText != null)
            freezeCounterText.text = $"Freezes: {freezeCount}/3";
        if (addTimeCounterText != null)
            addTimeCounterText.text = addTimeUsed ? "Add Time: Used" : "Add Time: 1/1";
    }

    private void UseHint()
    {
        if (isGameOver || hintCount <= 0 || currentQuestion.isTrueFalse) return;

        hintCount--;
        UpdatePowerUpCounters();
        if (hintCount <= 0) hintButton.interactable = false;

        Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };
        int removed = 0;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != currentQuestion.correctIndex && removed < 2)
            {
                buttons[i].gameObject.SetActive(false);
                removed++;
            }
        }

        Debug.Log($"Hint used! Remaining hints: {hintCount}");
    }

    private void UseFreezeTime()
    {
        if (isGameOver || freezeCount <= 0 || isTimeFrozen) return;

        freezeCount--;
        UpdatePowerUpCounters();
        if (freezeCount <= 0) freezeTimeButton.interactable = false;

        StartCoroutine(FreezeTimerCoroutine());
    }

    private System.Collections.IEnumerator FreezeTimerCoroutine()
    {
        isTimeFrozen = true;
        Color originalColor = timerImage.color;
        timerImage.color = Color.cyan;

        Debug.Log($"Time frozen for 5 seconds!");
        yield return new WaitForSecondsRealtime(5f);

        timerImage.color = originalColor;
        isTimeFrozen = false;
        Debug.Log("Time resumed!");
    }

    private void UseAddTime()
    {
        if (isGameOver || addTimeUsed) return;

        addTimeUsed = true;
        addTimeButton.interactable = false;
        timeRemaining += 5f;
        UpdatePowerUpCounters();
        Debug.Log("Added +5 seconds!");
    }
    private void ShowFunFactIfAvailable()
    {
        if (isGameOver) return;

        if (!string.IsNullOrEmpty(currentQuestion.funFact) && funFactPanel != null)
        {
            MultipleChoice.SetActive(false);
            TF.SetActive(false);
            SetPowerUpButtonsActive(false);

            funFactPanel.SetActive(true);

            // Set the title and fun fact text
            if (funFactTitleText != null)
                funFactTitleText.text = "Did You Know?";

            if (funFactText != null)
                funFactText.text = currentQuestion.funFact;

            Invoke(nameof(HideFunFactAndContinue), 5f);
        }
        else
        {
            Invoke(nameof(NextQuestion), 2f);
        }
    }

    private void HideFunFactAndContinue()
    {
        if (isGameOver) return;

        if (funFactPanel != null)
            funFactPanel.SetActive(false);

        SetPowerUpButtonsActive(true);
        isAnswered = false;
        NextQuestion();
    }

    private void SetPowerUpButtonsActive(bool state)
    {
        if (hintButton != null) hintButton.gameObject.SetActive(state);
        if (freezeTimeButton != null) freezeTimeButton.gameObject.SetActive(state);
        if (addTimeButton != null) addTimeButton.gameObject.SetActive(state);
        if (hintCounterText != null) hintCounterText.gameObject.SetActive(state);
        if (freezeCounterText != null) freezeCounterText.gameObject.SetActive(state);
        if (addTimeCounterText != null) addTimeCounterText.gameObject.SetActive(state);
    }
    private void NextQuestion()
    {
        if (isGameOver) return;

        currentIndex++;
        if (currentIndex < questions.Count)
        {
            funFactPanel.SetActive(false);
            StartQuestion();
        }
        else
        {
            QuizFinished();
        }
    }

        public static QuizManager Instance;
        private string currentPlayerId;
        private string currentCategory;
        private string currentDifficulty;

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

        public void SetCurrentGame(string category, string difficulty, string playerId)
        {
            currentCategory = category;
            currentDifficulty = difficulty;
            currentPlayerId = playerId;
        }

        public void EndGame(int finalScore)
        {
            LeaderboardManager leaderboard = FindAnyObjectByType<LeaderboardManager>();
            if (leaderboard != null)
            {
                leaderboard.SavePlayerScore(currentPlayerId, currentCategory, currentDifficulty, finalScore);
            }
        }





    private void QuizFinished()
    {
        Debug.Log("Quiz Finished! Score: " + score + "/" + questions.Count);

        
        if (!string.IsNullOrEmpty(GameSession.LoggedInUsername))
        {
            Debug.Log("=== ATTEMPTING TO SAVE SCORE ===");
            Debug.Log("Player: " + GameSession.LoggedInUsername);
            Debug.Log("Category: " + selectedCategory);
            Debug.Log("Difficulty: " + selectedDifficulty);
            Debug.Log("Score: " + score);

            
            ScoreManager.SaveScore(GameSession.LoggedInUsername, selectedCategory, selectedDifficulty, score);
            Debug.Log(" Score saved to SQLite database!");

            
            LeaderboardManager leaderboard = FindObjectOfType<LeaderboardManager>();
            if (leaderboard != null)
            {
                leaderboard.SavePlayerScore(GameSession.LoggedInUsername, selectedCategory, selectedDifficulty, score);
                Debug.Log(" Score also saved via LeaderboardManager!");
            }
        }
        else
        {
            Debug.LogWarning("No user logged in - score not saved");
        }

        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver(score, questions.Count);
        }
    }

    private void SaveScoreDirectly(string playerName, string category, string difficulty, int scoreValue)
    {
        try
        {
            string scoresFilePath = Path.Combine(Application.streamingAssetsPath, "player_scores.json");
            ScoreData scoreData;

            
            if (File.Exists(scoresFilePath))
            {
                string jsonData = File.ReadAllText(scoresFilePath);
                scoreData = JsonUtility.FromJson<ScoreData>(jsonData);
            }
            else
            {
                scoreData = new ScoreData();
            }

            
            PlayerScore newScore = new PlayerScore(playerName, category, difficulty, scoreValue);
            scoreData.AddOrUpdateScore(newScore);

            
            string updatedJson = JsonUtility.ToJson(scoreData, true);
            File.WriteAllText(scoresFilePath, updatedJson);

            Debug.Log("Score saved directly: " + playerName + " - " + scoreValue);
            Debug.Log("Total scores now: " + scoreData.scores.Count);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving score directly: " + e.Message);
        }
    }

}
    
    private void QuizFinished()
    {
        if (isGameOver) return;
        isGameOver = true;
        CancelInvoke();

        Debug.Log($"Quiz Finished! Final Score: {score}/{questions.Count}");

        GameOverUI gameOverUI = Object.FindFirstObjectByType<GameOverUI>();
        if (gameOverUI != null)
            gameOverUI.ShowGameOver(score, questions.Count);
    }
