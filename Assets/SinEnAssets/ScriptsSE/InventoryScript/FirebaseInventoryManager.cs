using UnityEngine;
using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseInventoryManager : MonoBehaviour
{
    public static FirebaseInventoryManager Instance;
    private DatabaseReference db;
    private FirebaseAuth auth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private bool IsAuthenticated() =>
        auth.CurrentUser != null &&
        auth.CurrentUser.UserId != "" &&
        auth.CurrentUser.UserId != null;

    private string CurrentUserId() => auth.CurrentUser.UserId;

    public void SaveInventory(List<Inventory> inventoryList, Action onSuccess, Action<string> onError)
    {
        Debug.Log("SaveInventory method called");

        if (!IsAuthenticated())
        {
            string error = "User not authenticated";
            Debug.LogError(error);
            onError(error);
            return;
        }

        string userId = CurrentUserId();
        Debug.Log($"Saving inventory for user: {userId}");

        // Clear existing inventory first
        db.Child("players").Child(userId).Child("inventoryItems").RemoveValueAsync()
            .ContinueWithOnMainThread(clearTask =>
            {
                if (clearTask.IsCanceled)
                {
                    onError("Clear task canceled");
                    return;
                }

                if (clearTask.IsFaulted)
                {
                    onError(clearTask.Exception?.ToString() ?? "Unknown error");
                    return;
                }

                Debug.Log("Old inventory cleared, now saving new items...");

                // Save each inventory item
                if (inventoryList.Count == 0)
                {
                    Debug.Log("No items to save");
                    onSuccess();
                    return;
                }

                Dictionary<string, object> updates = new Dictionary<string, object>();

                for (int i = 0; i < inventoryList.Count; i++)
                {
                    string json = JsonUtility.ToJson(inventoryList[i]);
                    Debug.Log($"Item {i} JSON: {json}");

                    // Parse JSON to object for Firebase
                    Dictionary<string, object> itemData = new Dictionary<string, object>
                    {
                        { "itemID", inventoryList[i].itemID },
                        { "collectibleDetails", new Dictionary<string, object>
                            {
                                { "collectibleID", inventoryList[i].collectibleDetails.collectibleID },
                                { "collectibleName", inventoryList[i].collectibleDetails.collectibleName },
                                { "tier", inventoryList[i].collectibleDetails.tier },
                                { "quantity", inventoryList[i].collectibleDetails.quantity }
                            }
                        }
                    };

                    updates[$"/players/{userId}/inventoryItems/item_{i}"] = itemData;
                }

                Debug.Log($"Updating {updates.Count} items in Firebase");

                db.UpdateChildrenAsync(updates)
                    .ContinueWithOnMainThread(updateTask =>
                    {
                        if (updateTask.IsCanceled)
                        {
                            onError("Update task canceled");
                            return;
                        }

                        if (updateTask.IsFaulted)
                        {
                            onError(updateTask.Exception?.ToString() ?? "Unknown error");
                            return;
                        }

                        Debug.Log("Inventory saved successfully to Firebase!");
                        onSuccess();
                    });
            });
    }


    public void LoadInventory(Action<List<Inventory>> onSuccess, Action<string> onError)
    {
        Debug.Log("LoadInventory method called");
        if (!IsAuthenticated())
        {
            onError("User not authenticated");
            return;
        }

        string userId = CurrentUserId();
        db.Child("players").Child(userId).Child("inventoryItems").GetValueAsync()
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
                    List<Inventory> inventoryList = new List<Inventory>();

                    if (snapshot.Exists)
                    {
                        foreach (DataSnapshot childSnapshot in snapshot.Children)
                        {
                            string json = childSnapshot.GetRawJsonValue();
                            Inventory inventory = JsonUtility.FromJson<Inventory>(json);
                            inventoryList.Add(inventory);
                        }

                        Debug.Log($"Inventory loaded: {inventoryList.Count} items");
                    }

                    onSuccess(inventoryList);
                }
            });
    }
}