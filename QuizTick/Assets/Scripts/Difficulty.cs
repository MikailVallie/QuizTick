using UnityEngine;
using UnityEngine.SceneManagement;
public class JAVA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        SceneManager.LoadSceneAsync("Diffulty Panel");
    }

    // Update is called once per frame
    public void Update()
    {
        Application.Quit();
    }
}
