using UnityEngine;
using System.Collections.Generic;

public class RandomObjectSpawner : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    public GameObject[] objectPrefabs;
    
    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera playerCamera;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private float horizontalSpread = 1.5f;
    [SerializeField] private float spawnHeight = 0.5f; // Relative to camera
    
    [Header("Orb Rotation")]
    [SerializeField] private bool rotateWithCamera = true;
    [SerializeField] private float fixedXRotation = 0f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool drawDebugGizmos = true;
    
    private List<GameObject> spawnedObjects = new List<GameObject>();
    
    void Start()
    {
        if (showDebugLogs) Debug.Log("RandomObjectSpawner Started");
        
        AutoFindReferences();
    }
    
    void AutoFindReferences()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("XR Origin")?.transform;
            if (playerTransform != null && showDebugLogs) 
                Debug.Log($"Auto-found Player Transform: {playerTransform.name}");
        }
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera != null && showDebugLogs) 
                Debug.Log($"Auto-found Player Camera: {playerCamera.name}");
        }
    }
    
    public void SpawnObjects()
    {
        if (showDebugLogs) Debug.Log("=== SPAWN OBJECTS CALLED ===");
        
        ClearSpawnedObjects();
        
        // Validate everything
        if (!ValidateSetup()) return;
        
        // Calculate positions relative to CAMERA
        Vector3[] spawnPositions = CalculateSpawnPositions();
        Quaternion spawnRotation = GetSpawnRotation();
        
        // Shuffle prefabs
        List<GameObject> prefabsToSpawn = new List<GameObject>(objectPrefabs);
        ShuffleList(prefabsToSpawn);
        
        // Spawn each prefab
        for (int i = 0; i < Mathf.Min(prefabsToSpawn.Count, 4); i++)
        {
            if (prefabsToSpawn[i] == null)
            {
                Debug.LogError($"ERROR: Prefab at index {i} is null!");
                continue;
            }
            
            GameObject spawnedObj = Instantiate(prefabsToSpawn[i], spawnPositions[i], spawnRotation);
            spawnedObjects.Add(spawnedObj);
            
            if (showDebugLogs)
            {
                Debug.Log($"Spawned {i}: {prefabsToSpawn[i].name} at {spawnPositions[i]}");
                Debug.DrawRay(spawnPositions[i], Vector3.up * 0.5f, Color.green, 3f);
            }
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"SUCCESS: Spawned {spawnedObjects.Count} orbs");
            Debug.Log($"Camera position: {playerCamera.transform.position}");
            Debug.Log($"Camera forward: {playerCamera.transform.forward}");
        }
    }
    
    private bool ValidateSetup()
    {
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogError("ERROR: No prefabs assigned!");
            return false;
        }
        
        if (objectPrefabs.Length < 4)
        {
            Debug.LogError($"ERROR: Need 4 prefabs, but only have {objectPrefabs.Length}");
            return false;
        }
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("ERROR: Player Camera is null!");
                return false;
            }
        }
        
        return true;
    }
    
    private Vector3[] CalculateSpawnPositions()
    {
        Vector3[] positions = new Vector3[4];
        
        // Use the CAMERA'S position and rotation (not playerTransform)
        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        
        // Make calculations horizontal only
        cameraForward.y = 0;
        cameraForward.Normalize();
        
        cameraRight.y = 0;
        cameraRight.Normalize();
        
        // Calculate base position: in front of camera at fixed height
        Vector3 basePosition = cameraPos + (cameraForward * spawnDistance);
        basePosition.y = cameraPos.y + spawnHeight; // Set height relative to camera
        
        if (showDebugLogs)
        {
            Debug.Log($"Camera Position: {cameraPos}");
            Debug.Log($"Camera Forward: {playerCamera.transform.forward}");
            Debug.Log($"Base Position: {basePosition}");
        }
        
        // Draw debug rays
        if (drawDebugGizmos)
        {
            Debug.DrawRay(cameraPos, cameraForward * spawnDistance, Color.blue, 3f);
            Debug.DrawRay(basePosition, Vector3.up * 0.5f, Color.red, 3f);
        }
        
        // Calculate 4 positions in a horizontal line
        for (int i = 0; i < 4; i++)
        {
            float offset = (i - 1.5f) * (horizontalSpread / 3f);
            positions[i] = basePosition + (cameraRight * offset);
            
            if (drawDebugGizmos)
            {
                Debug.DrawRay(positions[i], Vector3.up * 0.2f, Color.green, 3f);
            }
        }
        
        return positions;
    }
    
    private Quaternion GetSpawnRotation()
    {
        if (rotateWithCamera)
        {
            // Match camera's yaw (y-rotation) with fixed pitch (x-rotation)
            return Quaternion.Euler(fixedXRotation, playerCamera.transform.eulerAngles.y, 0f);
        }
        else
        {
            return Quaternion.Euler(fixedXRotation, 0f, 0f);
        }
    }
    
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    public void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
        
        if (showDebugLogs)
            Debug.Log("Cleared spawned objects");
    }
    
    // Visualize spawn positions in Scene view
    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos || playerCamera == null) return;
        
        Gizmos.color = Color.yellow;
        
        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        
        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();
        
        Vector3 basePosition = cameraPos + (cameraForward * spawnDistance);
        basePosition.y = cameraPos.y + spawnHeight;
        
        // Draw spawn points
        for (int i = 0; i < 4; i++)
        {
            float offset = (i - 1.5f) * (horizontalSpread / 3f);
            Vector3 spawnPos = basePosition + (cameraRight * offset);
            
            Gizmos.DrawWireSphere(spawnPos, 0.1f);
            Gizmos.DrawLine(spawnPos, spawnPos + Vector3.up * 0.2f);
        }
        
        // Draw line connecting them
        for (int i = 0; i < 3; i++)
        {
            float offset1 = (i - 1.5f) * (horizontalSpread / 3f);
            float offset2 = ((i + 1) - 1.5f) * (horizontalSpread / 3f);
            
            Vector3 pos1 = basePosition + (cameraRight * offset1);
            Vector3 pos2 = basePosition + (cameraRight * offset2);
            
            Gizmos.DrawLine(pos1, pos2);
        }
    }
}