using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float delay = 0.10f;  //Time between letters

    private TMP_Text textComponent;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>(); //Get the text component 
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        string fullText = textComponent.text;
        textComponent.text = ""; //Clear text at start

        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}


