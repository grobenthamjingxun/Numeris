using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;

public class SignUpUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private TMP_InputField emailField;
    [SerializeField]
    private TMP_InputField passwordField;
    [SerializeField]
    private TMP_InputField displayNameField;
    [SerializeField]
    private TMP_Text errorText;

    public void Signup()
    {
        // Obtain text from input fields
        var email = emailField.text;
        var password = passwordField.text;
        var displayName = displayNameField.text;

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
        if (displayName.Length == 0)
        {
            ShowError("Display name cannot be empty");
            return;
        }
        // Input validation
        if (!email.Contains("@") || !email.Contains("."))
        {
            ShowError("Empty or invalid e-mail address");
            return;
        }
        // TODO: More validations
        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters long");
            return;
        }
        else
        {
            ShowError(""); // Clear error
        }

        FirebaseAuth
            .DefaultInstance
            .CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    ShowError("Error signing up");
                    if (task.Exception != null) Debug.Log(task.Exception);
                    return;
                }

                FirebaseManager.Instance.SetDisplayName(displayName, ShowError, UIManager.Instance.ShowLogin);
            });
    }
    private void ShowError(string error)
    {
        errorText.text = error;
    }
}
