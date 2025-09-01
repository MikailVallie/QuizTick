using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    public void SelectCategory(string category)
    {
        GameData.Instance.SelectedCategory = category;
        Debug.Log("Category selected: " + category);
    }
}
