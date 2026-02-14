/*
* Author: Cheang Wei Cheng
* Date: 20/01/2026
* Description: This script is to be attached to the staff.
* It handles the logic for detecting when the player places an orb into the staff's socket and determining if it's the correct answer.
* It also manages feedback to the player, such as displaying "Correct!" or "Wrong!" text and playing particle effects for correct answers.
* Additionally, if the player selects a wrong answer, it triggers only the currently targeted enemy to start chasing the player.
*/

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;
using System.Collections;

public class AnswerDetection : MonoBehaviour
{
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private ParticleSystem correctAnswerEffect;
    [SerializeField] private TMP_Text questionText;
    private XRSocketInteractor socketInteractor;
    
    // Public property to check if correct orb is attached
    public bool IsCorrectOrbAttached { get; private set; } = false;

    [Header("Target-Specific Chase")]
    [SerializeField] private TargetSelector targetSelector; // Reference to target selector

    /// <summary>
    /// In Start(), listeners are set up for when an orb is placed into or removed from the staff's socket.
    /// The script also attempts to auto-assign references for the feedback text, question text, and target selector,
    /// since these variables are in a different scene from the staff and will not be assigned in the inspector.
    /// </summary>
    void Start()
    {
        // Look for unassigned values by name
        if (feedbackText == null)
        {
            feedbackText = GameObject.Find("Test Text").GetComponent<TMP_Text>();
        }
        if (questionText == null)
        {
            questionText = GameObject.Find("Question").GetComponent<TMP_Text>();
        }
        if (targetSelector == null)
        {
            targetSelector = GameObject.Find("XR Origin (XR Rig)").GetComponent<TargetSelector>();
            if (targetSelector != null)
            {
                Debug.Log($"Auto-found TargetSelector: {targetSelector.name}");
            }
            else
            {
                Debug.LogWarning("TargetSelector not found in scene. Targeted enemy chase will not work.");
            }
        }
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnSelectEntered);
        socketInteractor.selectExited.AddListener(OnSelectExited);
        feedbackText.text = "";
        correctAnswerEffect.Stop();
    }

    /// <summary>
    /// When an orb is placed into the staff's socket, this method checks if it's the correct answer by looking at the tag of the selected object.
    /// If it's correct, it updates the feedback text, plays the correct answer effect, and hides the wrong orbs and question text.
    /// If it's wrong, it updates the feedback text and triggers only the currently targeted enemy to start chasing the player, without affecting other enemies.
    /// </summary>
    /// <param name="args"></param>
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        GameObject selectedObject = args.interactableObject.transform.gameObject;

        if (selectedObject.CompareTag("CorrectOrb"))
        {
            Debug.Log("Correct answer selected!");
            AudioManager.Instance.PlayCorrectAnswer();
            feedbackText.text = "Correct!";
            // text disappears after a short delay
            StartCoroutine(HideText(2f));
            IsCorrectOrbAttached = true;
            if (correctAnswerEffect != null)
            {
                correctAnswerEffect.Play();
            }
            GameObject[] wrongOrbs = GameObject.FindGameObjectsWithTag("WrongOrb");
            foreach (GameObject orb in wrongOrbs)
            {
                orb.SetActive(false);
            }
            questionText.text = "";
        }
        else if (selectedObject.CompareTag("WrongOrb"))
        {
            Debug.Log("Wrong answer selected.");
            AudioManager.Instance.PlayWrongAnswer();
            feedbackText.text = "Wrong!";
            // text disappears after a short delay
            StartCoroutine(HideText(2f));
            IsCorrectOrbAttached = false;
            
            // Make only the TARGETED enemy chase
            TriggerTargetedEnemyChase();
        }
    }
    
    // Handle orb removal
    private void OnSelectExited(SelectExitEventArgs args)
    {
        IsCorrectOrbAttached = false;
        feedbackText.text = "";
    }

    /// <summary>
    /// This method is called when the player selects a wrong answer. It checks if there is a currently targeted enemy using the TargetSelector.
    /// If there is a target, it gets the PatrolChaseFSM component of that enemy and calls the OnInteractionFailed() method to make that specific enemy start chasing the player.
    /// If there is no target or if the targeted enemy does not have a PatrolChaseFSM component, it logs an error message.
    /// This ensures that only the currently targeted enemy reacts to the wrong answer, while other enemies remain unaffected.
    /// </summary>
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
    
    // Helper method to find player
    private Transform FindPlayerTransform()
    {
        // Look for XR player name
        name = "XR Origin (XR Rig)";
        
        GameObject player = GameObject.Find(name);
        if (player != null)
        {
            return player.transform;
        }
        
        // Fallback to camera
        if (Camera.main != null)
        {
            return Camera.main.transform;
        }
        
        Debug.LogError("Could not find player transform!");
        return null;
    }

    private IEnumerator HideText(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackText.text = "";
    }

    private void OnDestroy()
    {
        socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
        socketInteractor.selectExited.RemoveListener(OnSelectExited);
    }
}