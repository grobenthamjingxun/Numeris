using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChestSpawnOnOpen : MonoBehaviour
{
    public Animator chestAnimator;
    public string openTriggerName = "Open";

    public GameObject prefabToSpawn;
    public Transform spawnPoint;

    public GameObject chestRootObject; // the whole chest to destroy later

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    private GameObject spawnedObject;

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    void OnEnable()
    {
        socket.selectEntered.AddListener(OnSocketInserted);
    }

    void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnSocketInserted);
    }

    private void OnSocketInserted(SelectEnterEventArgs args)
    {
        // open animation
        if (chestAnimator != null)
            chestAnimator.SetTrigger(openTriggerName);

        // spawn prefab
        if (prefabToSpawn != null && spawnPoint != null && spawnedObject == null)
        {
            spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

            // give prefab reference to chest so it can destroy it
            DestroyChestOnGrab destroyScript = spawnedObject.GetComponent<DestroyChestOnGrab>();
            if (destroyScript != null)
            {
                destroyScript.chestToDestroy = chestRootObject;
            }
        }
    }
}
