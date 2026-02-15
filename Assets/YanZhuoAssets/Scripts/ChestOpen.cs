using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChestOpen : MonoBehaviour
{
  public Animator chestAnimator;
    public string openTriggerName = "Open";

    public GameObject openVFX;
    public Transform vfxSpawnPoint;

    public AudioSource openSound;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    void OnEnable()
    {
        socket.selectEntered.AddListener(OnItemInserted);
    }

    void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnItemInserted);
    }

    private void OnItemInserted(SelectEnterEventArgs args)
    {
        Debug.Log("Socket activated");
        // Play animation
        if (chestAnimator != null)
            chestAnimator.SetTrigger(openTriggerName);

        // Spawn VFX
        if (openVFX != null && vfxSpawnPoint != null)
            Instantiate(openVFX, vfxSpawnPoint.position, vfxSpawnPoint.rotation);

        // Play sound
        if (openSound != null)
            openSound.Play();
    }
}
