using UnityEngine;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    public QuizManager quizManager;
    public GameObject difficultyScreen; 

    public Button javaButton;
    public Button javascriptButton;
    public Button pythonButton;
    public Button sqlButton;
    public Button csharpButton;
    public Button backButton;

    void Start()
    {
        javaButton.onClick.AddListener(() => OnCategorySelected("JAVA"));
        javascriptButton.onClick.AddListener(() => OnCategorySelected("JAVASCRIPT"));
        pythonButton.onClick.AddListener(() => OnCategorySelected("PYTHON"));
        sqlButton.onClick.AddListener(() => OnCategorySelected("SQL"));
        csharpButton.onClick.AddListener(() => OnCategorySelected("CSHARP"));
        backButton.onClick.AddListener(OnBackButton);
    }

    private void OnCategorySelected(string category)
    {
        quizManager.selectedCategory = category;

        this.gameObject.SetActive(false);
        difficultyScreen.SetActive(true);
    }

    private void OnBackButton()
    {
        Debug.Log("Back button pressed");
    }
}