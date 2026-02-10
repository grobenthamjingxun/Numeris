using UnityEngine;
using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Threading.Tasks;

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

    #region Authentication
    private bool IsAuthenticated() =>
        auth.CurrentUser != null &&
        !string.IsNullOrEmpty(auth.CurrentUser.UserId);

    private string CurrentUserId() => auth.CurrentUser.UserId;
    #endregion

    #region Save Inventory
    public void SaveInventory(List<Inventory> inventoryList, Action onSuccess, Action<string> onError)
    {
        Debug.Log("SaveInventory method called");
        if (!IsAuthenticated())
        {
            HandleError("User not authenticated", onError);
            return;
        }
        string userId = CurrentUserId();
        Debug.Log($"Saving inventory for user: {userId}");
        ClearExistingInventory(userId, inventoryList, onSuccess, onError);
    }

    private void ClearExistingInventory(string userId, List<Inventory> inventoryList, Action onSuccess, Action<string> onError)
    {
        db.Child("players").Child(userId).Child("inventoryItems").RemoveValueAsync()
            .ContinueWithOnMainThread(clearTask =>
            {
                if (HandleTaskFailure(clearTask, "Clear", onError))
                {
                    return;
                }
                Debug.Log("Old inventory cleared, now saving new items...");
                SaveNewInventoryItems(userId, inventoryList, onSuccess, onError);
            });
    }

    private void SaveNewInventoryItems(string userId, List<Inventory> inventoryList, Action onSuccess, Action<string> onError)
    {
        if (inventoryList.Count == 0)
        {
            Debug.Log("No items to save");
            onSuccess();
            return;
        }
        Dictionary<string, object> updates = BuildInventoryUpdates(userId, inventoryList);
        Debug.Log($"Updating {updates.Count} items in Firebase");

        db.UpdateChildrenAsync(updates)
            .ContinueWithOnMainThread(updateTask =>
            {
                if (HandleTaskFailure(updateTask, "Update", onError))
                {
                    return;
                }
                Debug.Log("Inventory saved successfully to Firebase!");
                onSuccess();
            });
    }

    private Dictionary<string, object> BuildInventoryUpdates(string userId, List<Inventory> inventoryList)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        for (int i = 0; i < inventoryList.Count; i++)
        {
            string json = JsonUtility.ToJson(inventoryList[i]);
            Debug.Log($"Item {i} JSON: {json}");

            Dictionary<string, object> itemData = CreateItemData(inventoryList[i]);
            updates[$"/players/{userId}/inventoryItems/item_{i}"] = itemData;
        }
        return updates;
    }

    private Dictionary<string, object> CreateItemData(Inventory inventory)
    {
        return new Dictionary<string, object>
        {
            { "itemID", inventory.itemID },
            { "collectibleDetails", new Dictionary<string, object>
                {
                    { "collectibleID", inventory.collectibleDetails.collectibleID },
                    { "collectibleName", inventory.collectibleDetails.collectibleName },
                    { "tier", inventory.collectibleDetails.tier },
                    { "quantity", inventory.collectibleDetails.quantity }
                }
            }
        };
    }

    #endregion

    #region Load Inventory
    public void LoadInventory(Action<List<Inventory>> onSuccess, Action<string> onError)
    {
        Debug.Log("LoadInventory method called");
        if (!IsAuthenticated())
        {
            HandleError("User not authenticated", onError);
            return;
        }
        string userId = CurrentUserId();
        db.Child("players").Child(userId).Child("inventoryItems").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (HandleTaskFailure(task, "Load", onError))
                {
                    return;
                }
                if (task.IsCompleted)
                {
                    List<Inventory> inventoryList = ParseInventoryFromSnapshot(task.Result);
                    Debug.Log($"Inventory loaded: {inventoryList.Count} items");
                    onSuccess(inventoryList);
                }
            });
    }

    private List<Inventory> ParseInventoryFromSnapshot(DataSnapshot snapshot)
    {
        List<Inventory> inventoryList = new List<Inventory>();
        if (!snapshot.Exists)
        {
            return inventoryList;
        }
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            string json = childSnapshot.GetRawJsonValue();
            Inventory inventory = JsonUtility.FromJson<Inventory>(json);
            inventoryList.Add(inventory);
        }
        return inventoryList;
    }
    #endregion

    #region Error Handling
    private bool HandleTaskFailure(Task task, string operationType, Action<string> onError)
    {
        if (task.IsCanceled)
        {
            HandleError($"{operationType} task canceled", onError);
            return true;
        }
        if (task.IsFaulted)
        {
            string errorMessage = task.Exception?.ToString() ?? "Unknown error";
            HandleError(errorMessage, onError);
            return true;
        }
        return false;
    }

    private void HandleError(string error, Action<string> onError)
    {
        Debug.LogError(error);
        onError(error);
    }
    #endregion
}