using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DestroyChestOnGrab : MonoBehaviour
{
    public GameObject chestToDestroy;
    public GameObject keyToDestroy;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (chestToDestroy != null)
        {
            Destroy(chestToDestroy);
        }

         if (keyToDestroy != null)
        {
            Destroy(keyToDestroy);
        }
    }
}
