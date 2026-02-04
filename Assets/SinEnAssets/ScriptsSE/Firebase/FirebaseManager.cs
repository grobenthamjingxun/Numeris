using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

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

    // Create a new player in the database
    public void CreatePlayer(string username, string email, Action<string> onError, Action onSuccess)
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot create player when user is not logged in!");
            onError("User not authenticated");
            return;
        }

        Player newPlayer = new Player(username, email);
        newPlayer.isLoggedIn = true;

        string json = JsonUtility.ToJson(newPlayer);
        string userId = CurrentUserId();

        db.Child("players").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }

                Debug.Log("Player created successfully!");
                onSuccess();
            });
    }

    // Load player data from database
    public void LoadPlayer(Action<Player> onSuccess, Action<string> onError)
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot load player when user is not logged in!");
            onError("User not authenticated");
            return;
        }

        string userId = CurrentUserId();
        db.Child("players").Child(userId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }

                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string json = snapshot.GetRawJsonValue();
                        Player player = JsonUtility.FromJson<Player>(json);
                        Debug.Log("Player loaded: " + player.username);
                        onSuccess(player);
                    }
                    else
                    {
                        onError("Player data not found");
                    }
                }
            });
    }

    // Update player data
    public void UpdatePlayer(Player player, Action onSuccess, Action<string> onError)
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot update player when user is not logged in!");
            onError("User not authenticated");
            return;
        }
        string json = JsonUtility.ToJson(player);
        string userId = CurrentUserId();

        db.Child("players").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }

                Debug.Log("Player updated successfully!");
                onSuccess();
            });
    }

    // Update specific player field
    public void UpdatePlayerField(string fieldName, object value, Action onSuccess, Action<string> onError)
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot update field when user is not logged in!");
            onError("User not authenticated");
            return;
        }

        string userId = CurrentUserId();
        db.Child("players").Child(userId).Child(fieldName).SetValueAsync(value)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }

                Debug.Log($"{fieldName} updated successfully!");
                onSuccess();
            });
    }

    // Add item to inventory
    public void AddInventoryItem(Inventory item, Action onSuccess, Action<string> onError)
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot add item when user is not logged in!");
            onError("User not authenticated");
            return;
        }

        string userId = CurrentUserId();
        string itemKey = db.Child("players").Child(userId).Child("inventoryItems").Push().Key;

        string json = JsonUtility.ToJson(item);

        db.Child("players").Child(userId).Child("inventoryItems").Child(itemKey).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }

                Debug.Log("Inventory item added successfully!");
                onSuccess();
            });
    }

    public void SetDisplayName(string displayName, Action<string> onError, Action onSuccess)
    {
        if (!IsAuthenticated())
        {
            Debug.Log("Cannot set display name when user is not logged in!");
            return;
        }

        FirebaseDatabase.DefaultInstance.RootReference.Child("players").Child(CurrentUserId()).Child("displayName")
            .SetValueAsync(displayName).ContinueWithOnMainThread(task =>
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

    // Updated method to save entire player including inventory
    public void SaveCompletePlayerData(Player player, Action onSuccess, Action<string> onError)
    {
        if (!IsAuthenticated())
        {
            onError("User not authenticated");
            return;
        }

        string json = JsonUtility.ToJson(player);
        string userId = CurrentUserId();

        db.Child("players").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }

                Debug.Log("Complete player data saved successfully!");
                onSuccess();
            });
    }

    // Load entire player data including inventory
    public void LoadCompletePlayerData(Action<Player> onSuccess, Action<string> onError)
    {
        if (!IsAuthenticated())
        {
            onError("User not authenticated");
            return;
        }

        string userId = CurrentUserId();
        db.Child("players").Child(userId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string json = snapshot.GetRawJsonValue();
                        Player player = JsonUtility.FromJson<Player>(json);
                        Debug.Log($"Player loaded: {player.username} with {player.inventoryItems.Count} items");
                        onSuccess(player);
                    }
                    else
                    {
                        onError("Player data not found");
                    }
                }
            });
    }

    // Fetch all players from the database  
    public void FetchAllPlayers(Action<Dictionary<string, Player>> onSuccess, Action<string> onError)
    {
        db.Child("players").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null) Debug.LogError(task.Exception);
                    onError(task.Exception?.ToString() ?? "Unknown error");
                    return;
                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Dictionary<string, Player> players = new Dictionary<string, Player>();

                    if (!snapshot.Exists || !snapshot.HasChildren)
                    {
                        Debug.Log("No players found in database");
                        onSuccess(players);
                        return;
                    }

                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        try
                        {
                            string userId = childSnapshot.Key;
                            string json = childSnapshot.GetRawJsonValue();
                            Player player = JsonUtility.FromJson<Player>(json);

                            if (player != null && !string.IsNullOrEmpty(player.username))
                            {
                                players.Add(userId, player);
                            }
                            else
                            {
                                Debug.LogWarning($"Invalid player data for userId: {userId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error parsing player data: {ex.Message}");
                        }
                    }

                    Debug.Log($"Fetched {players.Count} players successfully!");
                    onSuccess(players);
                }
            });
    }
}
