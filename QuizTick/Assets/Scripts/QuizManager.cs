using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    private int score = 0;

    [Header("Timer Settings")]
    public Image timerImage;
    public float timePerQuestion = 15f;
    private float timeRemaining;
    private bool isAnswered = false;

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
        StartQuestion();
    }

    private void StartQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            Debug.Log("Quiz Finished!");
            QuizFinished();
            return;
        }

        currentQuestion = questions[currentIndex];
        ResetTimer();
        ShowQuestion();
    }

    private void ResetTimer()
    {
        timeRemaining = timePerQuestion;
        isAnswered = false;
        if (timerImage != null)
            timerImage.fillAmount = 1f;
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

            // Shuffle questions
            for (int i = 0; i < questions.Count; i++)
            {
                Question temp = questions[i];
                int randIndex = Random.Range(i, questions.Count);
                questions[i] = questions[randIndex];
                questions[randIndex] = temp;
            }

            // Limit to 20 questions 
            int maxQuestions = Mathf.Min(20, questions.Count);
            questions = questions.GetRange(0, maxQuestions);

            Debug.Log("Loaded " + questions.Count + " questions from " + fileName);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            questions = new List<Question>();
        }
    }

    private void Update()
    {
        if (!isAnswered && questions != null && questions.Count > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timerImage != null)
                timerImage.fillAmount = timeRemaining / timePerQuestion;

            if (timeRemaining <= 0f)
            {
                TimeOut();
            }
        }
    }

    private void TimeOut()
    {
        isAnswered = true;
        Debug.Log("Time's up!");

        HighlightCorrectAnswer();
        Invoke("NextQuestion", 1.5f);
    }

    private void HighlightCorrectAnswer()
    {
        Debug.Log("Correct answer was: " + currentQuestion.correctIndex);

        Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == currentQuestion.correctIndex)
                buttons[i].image.color = Color.green; 
        }

        if (currentQuestion.isTrueFalse)
        {
            if (currentQuestion.correctIndex == 0) TrueBtn.image.color = Color.green; 
            else FalseBtn.image.color = Color.green;                                  
        }
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
        currentQuestion = q;

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

                buttons[i].image.color = Color.white; 

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

            TrueBtn.image.color = Color.white; 
            FalseBtn.image.color = Color.white; 

            TrueBtn.onClick.RemoveAllListeners();
            FalseBtn.onClick.RemoveAllListeners();

            TrueBtn.onClick.AddListener(() => OnAnswerSelected(0));
            FalseBtn.onClick.AddListener(() => OnAnswerSelected(1));
        }
    }

    public void OnAnswerSelected(int selectedIndex)
    {
        if (isAnswered) return;
        isAnswered = true;

        Question q = questions[currentIndex];
        Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };

        if (!q.isTrueFalse)
        {
            if (selectedIndex == q.correctIndex)
            {
                Debug.Log("Correct!");
                score++;
                buttons[selectedIndex].image.color = Color.green;

                SoundManager.Instance.PlaySound("CorrectAnswer");
            }
            else
            {
                Debug.Log("Wrong!");
                buttons[selectedIndex].image.color = Color.red;
                buttons[q.correctIndex].image.color = Color.green;

                SoundManager.Instance.PlaySound("IncorrectAnswer");
            }
        }
        else
        {
            if (selectedIndex == q.correctIndex)
            {
                Debug.Log("Correct!");
                score++;
                if (selectedIndex == 0) TrueBtn.image.color = Color.green;
                else FalseBtn.image.color = Color.green;

                SoundManager.Instance.PlaySound("CorrectAnswer");
            }
            else
            {
                Debug.Log("Wrong!");
                if (selectedIndex == 0) TrueBtn.image.color = Color.red;
                else FalseBtn.image.color = Color.red;

                if (q.correctIndex == 0) TrueBtn.image.color = Color.green;
                else FalseBtn.image.color = Color.green;
                
                SoundManager.Instance.PlaySound("IncorrectAnswer");
            }
        }

        Invoke("NextQuestion", 1.5f);
    }

    private void NextQuestion()
    {
        currentIndex++;

        if (currentIndex < questions.Count)
        {
            StartQuestion();
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
