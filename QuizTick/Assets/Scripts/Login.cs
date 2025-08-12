using UnityEngine;
using TMPro;
using SQLite4Unity3d;
using System.IO;
using System.Linq;
using BCrypt.Net;

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
            Debug.Log("Please enter both username and password.");
            return;
        }

        using (var db = new SQLiteConnection(dbPath))
        {
            var user = db.Table<User>().FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                Debug.Log("User not found.");
                return;
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (isPasswordCorrect)
            {
                Debug.Log("Login successful! Welcome, " + user.FirstName);
                // You can load the next scene or show dashboard here
            }
            else
            {
                Debug.Log("Incorrect password.");
            }
        }
    }
}
