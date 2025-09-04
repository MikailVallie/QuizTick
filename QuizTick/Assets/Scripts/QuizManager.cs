using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    private int score = 0; // Tracks correct answers

    // === TIMER VARIABLES === //
    [Header("Timer Settings")]
    public Image timerImage; // Assign a UI Image (like a circular fill) in Inspector
    public float timePerQuestion = 15f; // Time limit for each question
    private float timeRemaining; // Tracks time left for current question
    private bool isAnswered = false; // Prevents timer from running after an answer is selected
    // === END TIMER VARIABLES === //

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options;
        public int correctIndex;
        public bool isTrueFalse;
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

    private void Start()
    {
        selectedCategory = GameData.Instance.SelectedCategory;
        selectedDifficulty = GameData.Instance.SelectedDifficulty;

        LoadQuestions();
        // ShowQuestion(); // We'll start the timer and show the question together now
        StartQuestion(); // New method to start the question and timer
    }

    // === NEW METHOD: Starts the question and timer === //
    private void StartQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            Debug.Log("Quiz Finished!");
            QuizFinished();
            return;
        }

        currentQuestion = questions[currentIndex];
        ResetTimer(); // Reset the timer for the new question
        ShowQuestion(); // Show the question UI
    }

    // === NEW METHOD: Resets the timer for a new question === //
    private void ResetTimer()
    {
        timeRemaining = timePerQuestion;
        isAnswered = false;
        if (timerImage != null)
            timerImage.fillAmount = 1f; // Reset UI to full
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

            for (int i = 0; i < questions.Count; i++)
            {
                Question temp = questions[i];
                int randIndex = Random.Range(i, questions.Count);
                questions[i] = questions[randIndex];
                questions[randIndex] = temp;
            }

            Debug.Log("Loaded " + questions.Count + " questions from " + fileName);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            questions = new List<Question>();
        }
    }

    // === UPDATE METHOD: Handles the timer countdown === //
    private void Update()
    {
        if (!isAnswered && questions != null && questions.Count > 0)
        {
            timeRemaining -= Time.deltaTime;

            // Update UI (if timerImage is assigned)
            if (timerImage != null)
                timerImage.fillAmount = timeRemaining / timePerQuestion;

            // Check if time is up
            if (timeRemaining <= 0f)
            {
                TimeOut();
            }
        }
    }

    // === NEW METHOD: Handles when time runs out === //
    private void TimeOut()
    {
        isAnswered = true;
        Debug.Log("Time's up!");

        // Show the correct answer
        HighlightCorrectAnswer();

        // Move to next question after a delay
        Invoke("NextQuestion", 1.5f); // 1.5 second delay to see the correct answer
    }

    // === NEW METHOD: Highlights the correct answer === //
    private void HighlightCorrectAnswer()
    {
        // This would need implementation based on your UI
        // For now, just log the correct answer
        Debug.Log("Correct answer was: " + currentQuestion.correctIndex);

        // You could add visual feedback here, like:
        // - Changing the color of the correct answer button to green
        // - Showing a "Time's Up!" message
    }

    [Header("Multiple Choice UI")]
    public GameObject MultipleChoice;
    public TextMeshProUGUI MCQuestion;
    public Button Option1Btn, Option2Btn, Option3Btn, Option4Btn;

    [Header("True/False UI")]
    public GameObject TF;
    public TextMeshProUGUI TFQuestion;
    public Button TrueBtn, FalseBtn;

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            Debug.Log("Quiz Finished!");
            QuizFinished();
            return;
        }

        Question q = questions[currentIndex];
        currentQuestion = q; // Store current question for timer reference

        if (!q.isTrueFalse)
        {
            MultipleChoice.SetActive(true);
            TF.SetActive(false);

            MCQuestion.text = q.questionText;
            Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(i < q.options.Length);
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
                int index = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
        }
        else
        {
            MultipleChoice.SetActive(false);
            TF.SetActive(true);

            TFQuestion.text = q.questionText;

            TrueBtn.onClick.RemoveAllListeners();
            FalseBtn.onClick.RemoveAllListeners();

            TrueBtn.onClick.AddListener(() => OnAnswerSelected(0));
            FalseBtn.onClick.AddListener(() => OnAnswerSelected(1));
        }
    }

    public void OnAnswerSelected(int selectedIndex)
    {
        if (isAnswered) return; // Prevent multiple answers
        isAnswered = true; // Stop the timer

        Question q = questions[currentIndex];

        // Check if the selected answer is correct
        if (selectedIndex == q.correctIndex)
        {
            Debug.Log("Correct!");
            score++;
        }
        else
        {
            Debug.Log("Wrong!");
        }

        // Show correct answer
        HighlightCorrectAnswer();

        // Move to next question after a delay
        Invoke("NextQuestion", 1.5f); // 1.5 second delay to see the result
    }

    // === NEW METHOD: Handles moving to the next question === //
    private void NextQuestion()
    {
        currentIndex++;

        if (currentIndex < questions.Count)
        {
            StartQuestion(); // Start next question with timer
        }
        else
        {
            QuizFinished();
        }
    }

    private void QuizFinished()
    {
        Debug.Log("Quiz Finished! Score: " + score + "/" + questions.Count);

        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver(score, questions.Count);
        }
    }
}
