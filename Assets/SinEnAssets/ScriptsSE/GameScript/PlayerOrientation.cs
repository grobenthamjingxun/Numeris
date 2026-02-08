/*
* Author: Kwek Sin En
* Date: 21/01/2026
* Description: Orients the player towards the current target smoothly.
*/

using UnityEngine;

public class PlayerOrientation : MonoBehaviour
{
    [SerializeField] private TargetSelector targetSelector;
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        if (targetSelector.CurrentTarget != null)
        {
            Vector3 directionToTarget = (targetSelector.CurrentTarget.transform.position - transform.position).normalized;
            directionToTarget.y = 0; // Keep rotation on the horizontal plane

            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}