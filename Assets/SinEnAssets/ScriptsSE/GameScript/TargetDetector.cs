/*
* Author: Kwek Sin En
* Date: 21/01/2026
* Description: Detects potential targets within a specified radius and field of view.
*/

using UnityEngine;
using System.Collections.Generic;

public class TargetDetector : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float fieldOfViewAngle = 90f; // Angle in degrees

    public List<GameObject> FindPotentialTargets()
    {
        List<GameObject> potentialTargets = new List<GameObject>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        Debug.Log($"OverlapSphere found: {hitColliders.Length} colliders");
        
        
        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToTarget = (hitCollider.transform.position - playerCameraTransform.position).normalized;
            float angle = Vector3.Angle(playerCameraTransform.forward, directionToTarget);
            
            // If enemy is within FOV cone (e.g., 90 degrees), add to list
            if (angle < fieldOfViewAngle / 2)
            {
                potentialTargets.Add(hitCollider.gameObject);
            }
        }
        
        return potentialTargets;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}