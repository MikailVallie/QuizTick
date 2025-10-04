using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public static class GameSession
{
    public static string LoggedInUsername = "";

    public static void SignOut()
    {
        LoggedInUsername = "";
        Debug.Log("User signed out.");
    }
}
