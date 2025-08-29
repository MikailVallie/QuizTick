using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
}
