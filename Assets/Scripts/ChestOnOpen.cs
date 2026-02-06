using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChestonOpen : MonoBehaviour
{
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
        // Spawn VFX
        if (openVFX != null && vfxSpawnPoint != null)
            Instantiate(openVFX, vfxSpawnPoint.position, vfxSpawnPoint.rotation);

        // Play sound
        if (openSound != null)
            openSound.Play();
    }
}