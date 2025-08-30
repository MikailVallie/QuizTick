using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{

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


    private string selectedCategory;
    private string selectedDifficulty;

    private void Start()
    {
        LoadQuestions();
        ShowQuestion();
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
            return;
        }

        Question q = questions[currentIndex];

        if (!q.isTrueFalse) 
        {
            MultipleChoice.SetActive(true);
            TF.SetActive(false);

            MCQuestion.text = q.questionText;
            Button[] buttons = { Option1Btn, Option2Btn, Option3Btn, Option4Btn };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(i < q.options.Length);
                buttons[i].GetComponentInChildren<TextMeshPro>().text = q.options[i];

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
        Question q = questions[currentIndex];

        // Check if the selected answer is correct
        if (selectedIndex == q.correctIndex)
        {
            Debug.Log("Correct!");
        }
        else
        {
            Debug.Log("Wrong!");
        }

        // Move to next question
        currentIndex++;
        ShowQuestion();
    }
}
