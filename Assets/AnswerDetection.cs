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

    void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnSelectEntered);
        feedbackText.text = "";
        correctAnswerEffect.Stop();
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        GameObject selectedObject = args.interactableObject.transform.gameObject;

        if (selectedObject.CompareTag("CorrectOrb"))
        {
            Debug.Log("Correct answer selected!");
            feedbackText.text = "Correct!";
            if (correctAnswerEffect != null)
            {
                correctAnswerEffect.Play();
            }
        }
        else if (selectedObject.CompareTag("WrongOrb"))
        {
            Debug.Log("Wrong answer selected.");
            feedbackText.text = "Wrong!";
        }
    }

    private void OnDestroy()
    {
        socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
    }
}