using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SQLite4Unity3d; 
using System.IO;
using System.Linq;
using BCrypt.Net;
using UnityEngine.SceneManagement; //to change scenes

public class Login : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    private string dbPath;

    void Start()
    {
        //full path
        dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");

        //open connection making sure user exists
        using (var db = new SQLiteConnection(dbPath))
        {
            db.CreateTable<User>();
        }
    }

    //called when the player presses the Login button
    public void OnLoginButtonPressed()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        //making sure both fields are filled
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowPopup("Please enter both username and password.");
            return;
        }
        
        //open the database and try to find the user
        using (var db = new SQLiteConnection(dbPath))
        {
            var user = db.Table<User>().FirstOrDefault(u => u.Username == username);

            //if no user exists with that username
            if (user == null)
            {
                ShowPopup("User not found!");
                return;
            }
            
            //verifying the password - the input password vs the hashed one in db
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!isPasswordCorrect)
            {
                ShowPopup("Incorrect password!");
                return;
            }
        }

        ShowPopup("Login successful!", 1.5f);

        //load Menu scene after short delay
        Invoke(nameof(LoadMenuScene), 1.5f);
    }

    private void LoadMenuScene()
    {
        SceneManager.LoadScene("MainMenu"); 
    }

        //popup method same as SignUp
    private void ShowPopup(string message, float duration = 2f)
    {
        GameObject popupObj = new GameObject("PopupMessage");
        var canvas = FindObjectOfType<Canvas>();

        //if no canvas in the scene, create one
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PopupCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        popupObj.transform.SetParent(canvas.transform, false);

        //background panel
        var bg = popupObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0f, 0f, 0f, 0.7f); // semi-transparent dark

        //unity's built-in sprite for rounded corners
        Sprite defaultSprite = UnityEngine.Resources.GetBuiltinResource<Sprite>("UISprite.psd");
        if (defaultSprite != null) bg.sprite = defaultSprite;
        bg.type = UnityEngine.UI.Image.Type.Sliced;

        RectTransform rect = popupObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(600, 120);
        rect.anchoredPosition = new Vector2(0, -Screen.height / 3); // bottom-center

        //add outline + shadow to panel
        var outline = popupObj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.5f);
        outline.effectDistance = new Vector2(4, -4);

        //text inside popup
        GameObject textObj = new GameObject("PopupText");
        textObj.transform.SetParent(popupObj.transform, false);
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 28;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;

        //green if success, red if error
        Color topColor = message.Contains("successful") ? new Color(0.1f, 0.9f, 0.1f) : new Color(1f, 0.3f, 0.3f);
        Color bottomColor = message.Contains("successful") ? new Color(0f, 0.5f, 0f) : new Color(0.6f, 0f, 0f);
        text.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.sizeDelta = rect.sizeDelta - new Vector2(40, 20);
        textRect.anchoredPosition = Vector2.zero;

        //a subtle text shadow
        var shadow = textObj.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.6f);
        shadow.effectDistance = new Vector2(2, -2);

        //animation controls fade + scale
        CanvasGroup group = popupObj.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        popupObj.transform.localScale = Vector3.one * 0.95f;

        StartCoroutine(AnimatePopup(popupObj, group, duration));
    }

    private System.Collections.IEnumerator AnimatePopup(GameObject popup, CanvasGroup group, float duration)
    {
        float t = 0f;

        //fade in + zoom in
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            float progress = t / 0.3f;
            group.alpha = Mathf.SmoothStep(0f, 1f, progress);
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(0.95f, 1f, progress);
            yield return null;
        }

        //stay on screen for given duration
        yield return new WaitForSeconds(duration);

        //fade out + zoom out
        t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            float progress = t / 0.3f;
            group.alpha = Mathf.SmoothStep(1f, 0f, progress);
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(1f, 0.95f, progress);
            yield return null;
        }

        Destroy(popup);
    }
}
