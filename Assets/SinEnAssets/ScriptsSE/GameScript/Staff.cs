using UnityEngine;

public class Staff : MonoBehaviour
{
    public Camera cam;
    public float speed = 10;
    public GameObject projectile; 
    public Transform activationPoint;
    public Vector3 destination;
    
    // ADDED: Reference to the AnswerDetection script
    public AnswerDetection answerDetection;
    
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }
    
    public void ShootProjectile()
    {
        // ADDED: Check if correct orb is attached
        if (answerDetection == null || !answerDetection.IsCorrectOrbAttached)
        {
            Debug.Log("Cannot shoot - no correct orb attached!");
            return;
        }
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            destination = hit.point;
        }
        else
        {
            destination = ray.GetPoint(1000);
        }
        InstantiateProjectile();
        AudioManager.Instance.PlayLaserBeam();
    }
    
    void InstantiateProjectile()
    {
        GameObject proj = Instantiate(projectile, activationPoint.position, Quaternion.identity);
        proj.GetComponent<Rigidbody>().linearVelocity = (destination - activationPoint.position).normalized * speed;
    }
}