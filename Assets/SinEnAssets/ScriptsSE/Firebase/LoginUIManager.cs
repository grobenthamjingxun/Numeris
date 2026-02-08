using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_InputField emailField;
    [SerializeField]
    private TMP_InputField passwordField;
    [SerializeField]
    private TMP_Text errorText;

    public void Login()
    {
        // Obtain text from input fields
        var email = emailField.text;
        var password = passwordField.text;

        // Input validation
        if (email.Length == 0)
        {
            ShowError("E-mail address cannot be empty");
            return;
        }
        if (password.Length == 0)
        {
            ShowError("Password cannot be empty");
            return;
        }
        if (!email.Contains("@") || !email.Contains("."))
        {
            ShowError("Empty or invalid e-mail address");
            return;
        }
        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters long");
            return;
        }
        else
        {
            ShowError(""); // Clear error
        }

        // Authenticate with Firebase
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    ShowError($"Login failed: {task.Exception?.GetBaseException().Message}");
                    return;
                }
                if (task.IsCanceled)
                {
                    ShowError("Login was cancelled.");
                    return;
                }
                if (task.IsCompleted)
                {
                    ShowError("");

                    // Set user as logged in
                    FirebaseManager.Instance.UpdatePlayerField("isLoggedIn", true,
                        onSuccess: () =>
                        {
                            Debug.Log("User marked as logged in");
                        },
                        onError: (error) =>
                        {
                            Debug.LogError("Failed to update login status: " + error);
                        }
                    );
                    FirebaseManager.Instance.LoadCompletePlayerData(
                        onSuccess: (player) =>
                        {
                            PlayerManager.Instance.SetPlayerData(player);
                            InvenManager.instance.LoadInventoryFromFirebase();
                            UIManager.Instance.ShowGame();
                        },
                        onError: (error) =>
                        {
                            Debug.LogError("Failed to load player data: " + error);
                            ShowError("Failed to load player data");
                        }
                    );
                }
            });
    }
    private void ShowError(string error)
    {
        errorText.text = error;
    }
}
