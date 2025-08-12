using UnityEngine;
using TMPro;
using SQLite4Unity3d;
using System.IO;
using BCrypt.Net;

public class SignUp : MonoBehaviour
{
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField emailInput;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField rePasswordInput;

    private string dbPath;

    void Start()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "users.db");

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

        if (password != rePassword)
        {
            Debug.Log("Passwords do not match!");
            return;
        }

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Username and password are required.");
            return;
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        using (var db = new SQLiteConnection(dbPath))
        {
            var existingUser = db.Table<User>().Where(u => u.Username == username).FirstOrDefault();
            if (existingUser != null)
            {
                Debug.Log("Username already taken.");
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

        Debug.Log("User registered successfully!");
    }
}

