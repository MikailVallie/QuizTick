using UnityEngine;
using UnityEngine.UI; 
using TMPro; //for input fields & text
using SQLite4Unity3d; //db
using System.IO; //to work with file paths
using System.Linq;
using BCrypt.Net; //for password hashing

public class SignUp : MonoBehaviour
{
    //attach in the inspector 
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
        //full path to the database file "users.db"
        dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");

        //confirming the User table exists (creates it if not already there)
        using (var db = new SQLiteConnection(dbPath))
        {
            db.CreateTable<User>();
        }
    }

    //when the SignUp button is clicked
    public void OnSignUpButtonPressed()
    {
        //grabing text from the input fields and cleaning up extra spaces
        string fname = firstNameInput.text.Trim();
        string lname = lastNameInput.text.Trim();
        string email = emailInput.text.Trim();
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;
        string rePassword = rePasswordInput.text;

        //check that both passwords match
        if (password != rePassword)
        {
            ShowPopup("Passwords do not match!");
            return;
        }

        //to ensure username and password aren’t empty
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowPopup("Username and password are required.");
            return;
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        using (var db = new SQLiteConnection(dbPath))
        {
            //first check if username is already in use
            var existingUser = db.Table<User>().Where(u => u.Username == username).FirstOrDefault();
            if (existingUser != null)
            {
                ShowPopup("Username already taken.");
                return;
            }

            //then check if email is already in use
            var existingEmail = db.Table<User>().Where(u => u.Email == email).FirstOrDefault();
            if (existingEmail != null)
            {
                ShowPopup("Email already registered.");
                return;
            }

            //both checks pass then create a new user object
            var newUser = new User()
            {
                FirstName = fname,
                LastName = lname,
                Email = email,
                Username = username,
                Password = hashedPassword
            };

             //inserting user into db
            db.Insert(newUser);
        }

        ShowPopup("User registered successfully!");

        // Switch panels after short delay so user sees the popup
        Invoke(nameof(SwitchToLogin), 1f);
    }

     // Switches the UI panels
    private void SwitchToLogin()
    {
        if (signUpPanel != null && loginPanel != null)
        {
            signUpPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
    }

    //toast notification - popup msg
    private void ShowPopup(string message, float duration = 2f)
    {
        GameObject popupObj = new GameObject("PopupMessage");
        var canvas = FindObjectOfType<Canvas>();

          //if there isnt a canvas that exists yet, create one
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
        bg.color = new Color(0f, 0f, 0f, 0.8f); // dark semi-transparent background
        bg.raycastTarget = false;

        //unity's built-in sprite for the background
        Sprite defaultSprite = UnityEngine.Resources.GetBuiltinResource<Sprite>("UISprite.psd");
        if (defaultSprite != null) bg.sprite = defaultSprite;
        bg.type = UnityEngine.UI.Image.Type.Sliced;

        RectTransform rect = popupObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(650, 140);
        rect.anchoredPosition = Vector2.zero;

        //text inside popup
        GameObject textObj = new GameObject("PopupText");
        textObj.transform.SetParent(popupObj.transform, false);
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;

         //green if success, red if error
        Color topColor = message.Contains("successfully") ? new Color(0.3f, 1f, 0.3f) : new Color(1f, 0.4f, 0.4f);
        Color bottomColor = message.Contains("successfully") ? new Color(0f, 0.6f, 0f) : new Color(0.6f, 0f, 0f);
        text.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.sizeDelta = rect.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;

        //a shadow for better visibility
        var shadow = textObj.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2, -2);

        //animation controls fade + scale
        CanvasGroup group = popupObj.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        popupObj.transform.localScale = Vector3.one * 0.8f;

        StartCoroutine(AnimatePopup(popupObj, group, duration));
    }

    //handles fade in/out + scale animation for the popup
    private System.Collections.IEnumerator AnimatePopup(GameObject popup, CanvasGroup group, float duration)
    {
        float t = 0f;

        //fade in + zoom in
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            float progress = t / 0.3f;
            group.alpha = Mathf.SmoothStep(0f, 1f, progress);
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(0.8f, 1f, progress);
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
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(1f, 0.8f, progress);
            yield return null;
        }

        Destroy(popup);
    }
}