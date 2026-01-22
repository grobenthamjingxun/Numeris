/*
* Author: Kwek Sin En
* Date: 21/01/2026
* Description: Handles target selection and cycling for lock-on mechanics in VR.
*/

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TargetSelector : MonoBehaviour
{
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float maxLockOnDistance = 25f;
    
    [Header("VR Input")]
    [SerializeField] private InputActionProperty cycleTargetAction; // Add this for VR

    public GameObject CurrentTarget { get; private set; }
    private List<GameObject> availableTargets = new List<GameObject>();
    private int currentTargetIndex = -1;

    private void Awake()
    {
        cycleTargetAction.action.Enable();
    }

    void Update()
    {
        // VR button to cycle targets (e.g., Y/B button on controller)
        if (cycleTargetAction.action.WasPressedThisFrame())
        {
            CycleTarget();
        }

        // Clear target if it's too far or destroyed
        if (CurrentTarget != null && 
            (Vector3.Distance(transform.position, CurrentTarget.transform.position) > maxLockOnDistance ||
             CurrentTarget == null || !CurrentTarget.activeSelf))
        {
            ClearTarget();
        }
    }

    public void CycleTarget()
    {
        Debug.Log("CycleTarget called!"); 
        
        availableTargets = targetDetector.FindPotentialTargets(); 
        
        Debug.Log($"Available targets after detection: {availableTargets.Count}");
        
        if (availableTargets.Count == 0)
        {
            ClearTarget();
            return;
        }

        // Remove current target if it's no longer available
        if (CurrentTarget != null && !availableTargets.Contains(CurrentTarget))
        {
            ClearTarget();
        }

        if (CurrentTarget == null)
        {
            // If no current target, select the one closest to the camera center
            CurrentTarget = GetClosestTargetToCameraCenter(availableTargets);
            if (CurrentTarget != null)
            {
                currentTargetIndex = availableTargets.IndexOf(CurrentTarget);
            }
        }
        else
        {
            // Cycle to the next target
            currentTargetIndex = (currentTargetIndex + 1) % availableTargets.Count;
            CurrentTarget = availableTargets[currentTargetIndex];
        }

        Debug.Log($"Locked on to: {CurrentTarget?.name}");
    }

    private GameObject GetClosestTargetToCameraCenter(List<GameObject> targets)
    {
        GameObject bestTarget = null;
        float minAngle = float.MaxValue;

        Camera cam = playerCameraTransform.GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("No camera found on playerCameraTransform!");
            return null;
        }

        foreach (var target in targets)
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(target.transform.position);
            // Check if target is in front of camera and within screen bounds
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
            {
                Vector3 directionToTarget = (target.transform.position - playerCameraTransform.position).normalized;
                float angle = Vector3.Angle(playerCameraTransform.forward, directionToTarget);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    bestTarget = target;
                }
            }
        }
        return bestTarget;
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
        currentTargetIndex = -1;
        Debug.Log("Target cleared.");
    }
}