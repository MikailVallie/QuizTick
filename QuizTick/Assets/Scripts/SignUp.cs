using UnityEngine;
using UnityEngine.UI;   
using TMPro;
using SQLite4Unity3d;
using System.IO;
using System.Linq;
using BCrypt.Net;

public class SignUp : MonoBehaviour
{
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField emailInput;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField rePasswordInput;

    public GameObject signUpPanel; 
    public GameObject loginPanel;  

    private string dbPath;

    void Start()
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");

        using (var db = new SQLiteConnection(dbPath))
        {
            db.CreateTable<User>();
        }
    }

    public void OnSignUpButtonPressed()
    {
        string fname = firstNameInput.text.Trim();
        string lname = lastNameInput.text.Trim();
        string email = emailInput.text.Trim();
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;
        string rePassword = rePasswordInput.text;

        // Check passwords
        if (password != rePassword)
        {
            ShowPopup("Passwords do not match!");
            return;
        }

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowPopup("Username and password are required.");
            return;
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        using (var db = new SQLiteConnection(dbPath))
        {
            var existingUser = db.Table<User>().Where(u => u.Username == username).FirstOrDefault();
            if (existingUser != null)
            {
                ShowPopup("Username already taken.");
                return;
            }

            var newUser = new User()
            {
                FirstName = fname,
                LastName = lname,
                Email = email,
                Username = username,
                Password = hashedPassword
            };

            db.Insert(newUser);
        }

        ShowPopup("User registered successfully!");

        // Switch panels after short delay so user sees the popup
        Invoke(nameof(SwitchToLogin), 1f);
    }

    private void SwitchToLogin()
    {
        if (signUpPanel != null && loginPanel != null)
        {
            signUpPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
    }

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

        // Background panel
        var bg = popupObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0f, 0f, 0f, 0.8f); // dark semi-transparent
        bg.raycastTarget = false;

        Sprite defaultSprite = UnityEngine.Resources.GetBuiltinResource<Sprite>("UISprite.psd");
        if (defaultSprite != null) bg.sprite = defaultSprite;
        bg.type = UnityEngine.UI.Image.Type.Sliced;

        RectTransform rect = popupObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(650, 140);
        rect.anchoredPosition = Vector2.zero;

        // Text
        GameObject textObj = new GameObject("PopupText");
        textObj.transform.SetParent(popupObj.transform, false);
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;

        // Gradient effect by vertex colors
        Color topColor = message.Contains("successfully") ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.4f, 0.4f);
        Color bottomColor = message.Contains("successfully") ? new Color(0f, 0.6f, 0f) : new Color(0.6f, 0f, 0f);
        text.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.sizeDelta = rect.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;

        // Shadow effect
        var shadow = textObj.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2, -2);

        // Animation: fade + scale
        CanvasGroup group = popupObj.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        popupObj.transform.localScale = Vector3.one * 0.8f;

        StartCoroutine(AnimatePopup(popupObj, group, duration));
    }

    // Coroutine for fade/scale animation
    private System.Collections.IEnumerator AnimatePopup(GameObject popup, CanvasGroup group, float duration)
    {
        float t = 0f;

        // Fade in + scale up
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            float progress = t / 0.3f;
            group.alpha = Mathf.SmoothStep(0f, 1f, progress);
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(0.8f, 1f, progress);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        // Fade out + scale down
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