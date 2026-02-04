using System.Collections.Generic;
using UnityEngine;

public class FiftyFiftyPowerUp : MonoBehaviour
{
    [Header("Orb Tags (must match your prefabs)")]
    public string wrongOrbTag = "WrongOrb";
    public int removeCount = 2;

    [Header("Optional: prevent removing orbs too far away (VR safety)")]
    public bool useDistanceFilter = true;
    public float maxDistanceFromCamera = 6f;

    public void Activate()
    {
        GameObject[] wrongOrbs = GameObject.FindGameObjectsWithTag(wrongOrbTag);

        if (wrongOrbs == null || wrongOrbs.Length == 0)
        {
            Debug.LogWarning("[50:50] No objects found with tag 'WrongOrb'. Check your prefab tags.");
            return;
        }

        Camera cam = Camera.main;

        List<GameObject> candidates = new List<GameObject>();

        foreach (GameObject orb in wrongOrbs)
        {
            if (orb == null) continue;
            if (!orb.activeInHierarchy) continue;

            if (useDistanceFilter && cam != null)
            {
                float d = Vector3.Distance(cam.transform.position, orb.transform.position);
                if (d > maxDistanceFromCamera) continue;
            }

            candidates.Add(orb);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("[50:50] Found WrongOrbs but none passed filters (distance/active).");
            return;
        }

        int toRemove = Mathf.Min(removeCount, candidates.Count);

        for (int i = 0; i < toRemove; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            GameObject target = candidates[idx];
            candidates.RemoveAt(idx);

            if (target != null)
                target.SetActive(false);
        }

        Debug.Log($"[50:50] Disabled {toRemove} wrong orb(s).");
    }
}
