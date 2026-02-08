/*
* Author: Kwek Sin En
* Date: 21/01/2026
* Description: Manages the lock-on camera behavior, including target highlighting in VR.
*/

using UnityEngine;

public class LockOnCamera : MonoBehaviour
{
    [SerializeField] private TargetSelector targetSelector;
    [SerializeField] private Transform vrCamera; // Your VR camera (Main Camera under XR Origin)
    
    [Header("Target Highlight")]
    [SerializeField] private bool highlightTarget = true;
    [SerializeField] private Material highlightMaterial;
    
    private GameObject currentReticle;
    private Material originalMaterial;
    private Renderer currentTargetRenderer;


    void Update()
    {
        if (targetSelector.CurrentTarget != null)
        {
            // Highlight the target
            if (highlightTarget && highlightMaterial != null)
            {
                Renderer targetRenderer = targetSelector.CurrentTarget.GetComponent<Renderer>();
                if (targetRenderer != null && currentTargetRenderer != targetRenderer)
                {
                    // Restore previous target's material
                    RestoreOriginalMaterial();
                    
                    // Save and apply highlight to new target
                    currentTargetRenderer = targetRenderer;
                    originalMaterial = targetRenderer.material;
                    targetRenderer.material = highlightMaterial;
                }
            }
        }
        else
        {
            // Hide reticle when no target
            if (currentReticle != null)
            {
                currentReticle.SetActive(false);
            }
            
            // Restore original material
            RestoreOriginalMaterial();
        }
    }

    private void RestoreOriginalMaterial()
    {
        if (currentTargetRenderer != null && originalMaterial != null)
        {
            currentTargetRenderer.material = originalMaterial;
            currentTargetRenderer = null;
            originalMaterial = null;
        }
    }

    void OnDestroy()
    {
        RestoreOriginalMaterial();
        if (currentReticle != null)
        {
            Destroy(currentReticle);
        }
    }
}