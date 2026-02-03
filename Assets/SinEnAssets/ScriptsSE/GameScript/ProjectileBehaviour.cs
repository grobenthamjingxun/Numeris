using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public GameObject impactVFX;
    public int damage = 1;
    private bool hasCollided;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !hasCollided)
        {
            hasCollided = true;
            GameObject impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity);
            Destroy(impact, 2f);
            EnemyBehaviour enemy = collision.gameObject.GetComponent<EnemyBehaviour>();
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }
}
