using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SQLite4Unity3d;
using System.IO;
using System.Linq;
using BCrypt.Net;
using UnityEngine.SceneManagement; // Needed for scene loading

public class Login : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    private string dbPath;

    void Start()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "users.db");

        using (var db = new SQLiteConnection(dbPath))
        {
            db.CreateTable<User>(); // Ensure table exists
        }
    }

    public void OnLoginButtonPressed()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowPopup("Please enter both username and password.");
            return;
        }

        using (var db = new SQLiteConnection(dbPath))
        {
            var user = db.Table<User>().FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ShowPopup("User not found!");
                return;
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!isPasswordCorrect)
            {
                ShowPopup("Incorrect password!");
                return;
            }
        }

        ShowPopup("Login successful!", 1.5f);

        // Load Menu scene after short delay
        Invoke(nameof(LoadMenuScene), 1.5f);
    }

    private void LoadMenuScene()
    {
        SceneManager.LoadScene("MainMenu"); // Replace with your scene name
    }

    // Popup method same as SignUp
    private void ShowPopup(string message, float duration = 2f)
    {
        GameObject popupObj = new GameObject("PopupMessage");
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PopupCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        popupObj.transform.SetParent(canvas.transform, false);

        var bg = popupObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0f, 0f, 0f, 0.8f);
        bg.raycastTarget = false;

        Sprite defaultSprite = UnityEngine.Resources.GetBuiltinResource<Sprite>("UISprite.psd");
        if (defaultSprite != null) bg.sprite = defaultSprite;
        bg.type = UnityEngine.UI.Image.Type.Sliced;

        RectTransform rect = popupObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(650, 140);
        rect.anchoredPosition = Vector2.zero;

        GameObject textObj = new GameObject("PopupText");
        textObj.transform.SetParent(popupObj.transform, false);
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;

        Color topColor = message.Contains("successful") ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.4f, 0.4f);
        Color bottomColor = message.Contains("successful") ? new Color(0f, 0.6f, 0f) : new Color(0.6f, 0f, 0f);
        text.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.sizeDelta = rect.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;

        var shadow = textObj.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2, -2);

        CanvasGroup group = popupObj.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        popupObj.transform.localScale = Vector3.one * 0.8f;

        StartCoroutine(AnimatePopup(popupObj, group, duration));
    }

    private System.Collections.IEnumerator AnimatePopup(GameObject popup, CanvasGroup group, float duration)
    {
        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            float progress = t / 0.3f;
            group.alpha = Mathf.SmoothStep(0f, 1f, progress);
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(0.8f, 1f, progress);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            float progress = t / 0.3f;
            group.alpha = Mathf.SmoothStep(1f, 0f, progress);
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(1f, 0.8f, progress);
            yield return null;
        }

        Destroy(popup);
    }
}
