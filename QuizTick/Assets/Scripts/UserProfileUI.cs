using UnityEngine;
using TMPro;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class UserProfileUI : MonoBehaviour
{
    [Header("Input Fields (assign top-level TMP_InputField in Inspector)")]
    public TMP_InputField usernameInput;
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField emailInput;

    private string dbPath;

    void Awake()
    {
        // Ensure database path is set before OnEnable
        dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");
    }

    void OnEnable()
    {
        ShowUserInfo();
    }

    public void ShowUserInfo()
    {
        // Debug references
        if (usernameInput == null || firstNameInput == null || lastNameInput == null || emailInput == null)
        {
            Debug.LogError("One or more TMP_InputField references are not assigned in the Inspector!");
            return;
        }

        if (string.IsNullOrEmpty(GameSession.LoggedInUsername))
        {
            Debug.LogWarning("No logged-in user set in GameSession.");
            usernameInput.text = "Not logged in";
            firstNameInput.text = "-";
            lastNameInput.text = "-";
            emailInput.text = "-";
            return;
        }

        if (!File.Exists(dbPath))
        {
            Debug.LogError("Database not found at: " + dbPath);
            usernameInput.text = "DB missing";
            firstNameInput.text = "-";
            lastNameInput.text = "-";
            emailInput.text = "-";
            return;
        }

        using (var db = new SQLiteConnection(dbPath))
        {
            var foundUser = db.Table<User>().FirstOrDefault(u => u.Username == GameSession.LoggedInUsername);

            if (foundUser != null)
            {
                Debug.Log($"User found: {foundUser.FirstName} {foundUser.LastName}, {foundUser.Email}");

                // Set the InputField text
                usernameInput.text  = foundUser.Username;
                firstNameInput.text = foundUser.FirstName;
                lastNameInput.text  = foundUser.LastName;
                emailInput.text     = foundUser.Email;
            }
            else
            {
                Debug.LogError("User not found in DB: " + GameSession.LoggedInUsername);

                usernameInput.text = "User not found";
                firstNameInput.text = "-";
                lastNameInput.text = "-";
                emailInput.text = "-";
            }
        }
    }
}

