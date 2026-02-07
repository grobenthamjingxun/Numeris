using UnityEngine;

public class VRSoftFollowUI : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Drag your Main Camera here.")]
    [SerializeField] public Transform vrCamera;
    
    [Tooltip("Distance in meters from the user's face.")]
    public float distance = 0.5f;

    [Tooltip("Lower = faster follow; Higher = lazier/smoother follow.")]
    public float smoothTime = 0.3f;

    [Header("Behavior")]
    [Tooltip("If true, the UI stays level and won't tilt with your head.")]
    public bool lockPitch = true;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if (vrCamera == null) return;

        // 1. Calculate the target position in front of the camera
        Vector3 targetPosition = vrCamera.position + (vrCamera.forward * distance);

        // 2. Smoothly move toward that position using SmoothDamp (best for VR follow)
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime/4f);

        // 3. Determine the rotation (facing the user)
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - vrCamera.position);

        if (lockPitch)
        {
            // Keep the UI vertically upright even if the user tilts their head sideways
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }

        // 4. Smoothly rotate toward the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * (1f / smoothTime));
    }
}