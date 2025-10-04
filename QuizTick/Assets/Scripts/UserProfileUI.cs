using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class UserProfileUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField emailInput;

    [Header("Buttons")]
    public Button editButton;
    public Button saveButton;

    private string dbPath;

    void Awake()
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");

        SetFieldsEditable(false);
        saveButton.gameObject.SetActive(false);

        editButton.onClick.AddListener(OnEditClicked);
        saveButton.onClick.AddListener(OnSaveClicked);

        usernameInput.interactable = false;
    }

    void OnEnable()
    {
        ShowUserInfo();
    }

    void SetFieldsEditable(bool editable)
    {
        usernameInput.interactable = false;

        firstNameInput.interactable = editable;
        lastNameInput.interactable = editable;
        emailInput.interactable = editable;
    }

    void OnEditClicked()
    {
        SetFieldsEditable(true);
        saveButton.gameObject.SetActive(true);  
        editButton.gameObject.SetActive(false); 
    }

    void OnSaveClicked()
    {
        SetFieldsEditable(false);
        saveButton.gameObject.SetActive(false);
        editButton.gameObject.SetActive(true); 

        if (!File.Exists(dbPath))
        {
            Debug.LogError("Database not found at: " + dbPath);
            return;
        }

        using (var db = new SQLiteConnection(dbPath))
        {
            var user = db.Table<User>().FirstOrDefault(u => u.Username == usernameInput.text);

            if (user != null)
            {
                user.FirstName = firstNameInput.text;
                user.LastName = lastNameInput.text;
                user.Email = emailInput.text;

                db.Update(user);
                Debug.Log("User profile updated successfully.");
            }
            else
            {
                Debug.LogError("User not found in DB: " + usernameInput.text);
            }
        }
    }

    public void ShowUserInfo()
    {
        if (string.IsNullOrEmpty(GameSession.LoggedInUsername))
        {
            usernameInput.text = "Not logged in";
            firstNameInput.text = "-";
            lastNameInput.text = "-";
            emailInput.text = "-";
            return;
        }

        if (!File.Exists(dbPath))
        {
            Debug.LogError("Database not found at: " + dbPath);
            return;
        }

        using (var db = new SQLiteConnection(dbPath))
        {
            var foundUser = db.Table<User>().FirstOrDefault(u => u.Username == GameSession.LoggedInUsername);

            if (foundUser != null)
            {
                usernameInput.text  = foundUser.Username;
                firstNameInput.text = foundUser.FirstName;
                lastNameInput.text  = foundUser.LastName;
                emailInput.text     = foundUser.Email;
            }
        }
    }
}

