using UnityEngine;
using System.Collections.Generic;

public class RandomObjectSpawner : MonoBehaviour
{
    [Header("Prefabs to Spawn (one of each)")]
    public GameObject[] objectPrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to UniqueSpawner!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned to UniqueSpawner!");
            return;
        }

        // Create a list of available spawn points
        List<Transform> availableSpots = new List<Transform>(spawnPoints);

        // Shuffle the prefab list for random order
        List<GameObject> prefabsToSpawn = new List<GameObject>(objectPrefabs);
        ShuffleList(prefabsToSpawn);

        // Spawn each prefab at a random available spawn point
        foreach (GameObject prefab in prefabsToSpawn)
        {
            if (availableSpots.Count == 0)
            {
                Debug.LogWarning("Not enough spawn points for all prefabs!");
                break;
            }

            // Pick a random spawn point
            int index = Random.Range(0, availableSpots.Count);
            Transform spawnPoint = availableSpots[index];

            // Instantiate the prefab
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

            // Remove the used spawn point
            availableSpots.RemoveAt(index);
        }
    }

    // Utility function to shuffle a list
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
}
