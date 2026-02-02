using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RayEnemy : MonoBehaviour
{
    public string requiredTag = "Enemy";
    public int sceneIndexToLoad;
    public float maxDistance = 50f;

    public InputActionReference selectAction;

    public Color glowColor = Color.red;
    public float glowIntensity = 2f;

    private Renderer currentRenderer;
    private Material currentMaterial;

    void OnEnable()
    {
        selectAction.action.Enable();
    }

    void OnDisable()
    {
        selectAction.action.Disable();
        ClearGlow();
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance) &&
            hit.collider.CompareTag(requiredTag))
        {
            SetGlow(hit.collider);

            if (selectAction.action.WasPressedThisFrame())
            {
                SceneManager.LoadScene(sceneIndexToLoad);
            }
        }
        else
        {
            ClearGlow();
        }
    }

    void SetGlow(Collider col)
    {
        Renderer rend = col.GetComponent<Renderer>();
        if (rend == null || rend == currentRenderer) return;

        ClearGlow();

        currentRenderer = rend;
        currentMaterial = currentRenderer.material;

        currentMaterial.EnableKeyword("_EMISSION");
        currentMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
    }

    void ClearGlow()
    {
        if (currentMaterial != null)
        {
            currentMaterial.SetColor("_EmissionColor", Color.black);
        }

        currentRenderer = null;
        currentMaterial = null;
    }
}
