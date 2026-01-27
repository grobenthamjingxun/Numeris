using UnityEngine;
using System;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    private FirebaseAuth auth;
    private DatabaseReference db;
    private bool isFirebaseInitialized = false;
    public string DisplayName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseDatabase.DefaultInstance.RootReference;
        isFirebaseInitialized = true;
        Debug.Log("Firebase initialized");
    }

    private bool IsAuthenticated() =>
        auth.CurrentUser.UserId != "" &&
        auth.CurrentUser.UserId != null;

    private string CurrentUserId() =>
        auth.CurrentUser.UserId;
    
    public void SetDisplayName(string displayName, Action<string> onError, Action onSuccess)
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot set display name when user is not logged in!");
            return;
        }

        FirebaseDatabase
            .DefaultInstance
            .RootReference
            .Child("players")
            .Child(CurrentUserId())
            .Child("displayName")
            .SetValueAsync(displayName)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.Log(task.Exception);
                    onError(task.Exception.ToString());
                    return;
                }

                onSuccess();
            });
    }

    public void GetDisplayName()
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot set display name when user is not logged in!");
            return;
        }

        string userId = CurrentUserId();
        db.Child("players").Child(userId).Child("displayName").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (task.Exception != null) Debug.Log(task.Exception);
                return;
            }
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DisplayName = snapshot.Value.ToString();
                Debug.Log("Display name retrieved: " + DisplayName);
            }
        });
    }
}
