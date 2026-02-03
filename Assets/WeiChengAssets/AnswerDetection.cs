/// to be attached to the staff (not the orbs)
/// there are 4 possible orbs with one correct answer
/// the orbs each have a tag; the correct orb has the tag "CorrectOrb" whereas the incorrect orbs have the tag "WrongOrb"
/// when the player slots an orb into the staff, this script will check if it's the correct one
/// all of the objects are xrgrab interactable objects with the staff having an xr socket interactor component

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

public class AnswerDetection : MonoBehaviour
{
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private ParticleSystem correctAnswerEffect;
    private XRSocketInteractor socketInteractor;
    
    // ADDED: Public property to check if correct orb is attached
    public bool IsCorrectOrbAttached { get; private set; } = false;

    // ADDED: Target-Specific Chase
    [Header("Target-Specific Chase")]
    [SerializeField] private TargetSelector targetSelector; // Reference to your target selector

    void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnSelectEntered);
        socketInteractor.selectExited.AddListener(OnSelectExited); // ADDED
        feedbackText.text = "";
        correctAnswerEffect.Stop();
        
        // ADDED: Try to auto-find target selector
        if (targetSelector == null)
        {
            targetSelector = FindFirstObjectByType<TargetSelector>();
            if (targetSelector != null)
            {
                Debug.Log($"Auto-found TargetSelector: {targetSelector.name}");
            }
            else
            {
                Debug.LogWarning("TargetSelector not found in scene. Targeted enemy chase will not work.");
            }
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        GameObject selectedObject = args.interactableObject.transform.gameObject;

        if (selectedObject.CompareTag("CorrectOrb"))
        {
            Debug.Log("Correct answer selected!");
            feedbackText.text = "Correct!";
            IsCorrectOrbAttached = true; // ADDED
            if (correctAnswerEffect != null)
            {
                correctAnswerEffect.Play();
            }
        }
        else if (selectedObject.CompareTag("WrongOrb"))
        {
            Debug.Log("Wrong answer selected.");
            feedbackText.text = "Wrong!";
            IsCorrectOrbAttached = false; // ADDED
            
            // ADDED: Make only the TARGETED enemy chase
            TriggerTargetedEnemyChase();
        }
    }
    
    // ADDED: Handle orb removal
    private void OnSelectExited(SelectExitEventArgs args)
    {
        IsCorrectOrbAttached = false;
        feedbackText.text = "";
    }

    // ADDED: Only chase with currently targeted enemy
    private void TriggerTargetedEnemyChase()
    {
        if (targetSelector == null)
        {
            Debug.LogError("TargetSelector not found! Enemy will not chase.");
            return;
        }
        
        GameObject currentTarget = targetSelector.CurrentTarget;
        if (currentTarget == null)
        {
            Debug.LogWarning("No enemy currently targeted!");
            return;
        }
        
        PatrolChaseFSM chaser = currentTarget.GetComponent<PatrolChaseFSM>();
        if (chaser != null)
        {
            Transform playerTransform = FindPlayerTransform();
            if (playerTransform != null)
            {
                chaser.OnInteractionFailed(playerTransform);
                Debug.Log($"Targeted enemy {currentTarget.name} is now chasing!");
            }
            else
            {
                Debug.LogError("Could not find player transform!");
            }
        }
        else
        {
            Debug.LogError($"Targeted enemy {currentTarget.name} has no PatrolChaseFSM component!");
        }
    }
    
    // ADDED: Helper method to find player
    private Transform FindPlayerTransform()
    {
        if (Camera.main != null)
        {
            return Camera.main.transform;
        }
        
        Debug.LogError("Could not find player transform!");
        return null;
    }

    private void OnDestroy()
    {
        socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
        socketInteractor.selectExited.RemoveListener(OnSelectExited); // ADDED
    }
}