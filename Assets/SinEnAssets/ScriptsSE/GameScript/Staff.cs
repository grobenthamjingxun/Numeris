using UnityEngine;

public class Staff : MonoBehaviour
{
    public Camera cam;
    public float speed = 10;
    public GameObject projectile; 
    public Transform activationPoint;
    public Vector3 destination;
    
    public void ShootProjectile()
    {
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
    }
    void InstantiateProjectile()
    {
        GameObject proj = Instantiate(projectile, activationPoint.position, Quaternion.identity);
        proj.GetComponent<Rigidbody>().linearVelocity = (destination - activationPoint.position).normalized * speed;
    }
}
