using UnityEngine;

public class Explosion : MonoBehaviour
{
    //public float duration = 0.3f;
    public float explosionRadius = 1.5f;

    private void Start()
    {
        //Detect player in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player1"))
            {
                var health = hit.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.RegisterHit();
                }
            }
        }
        Destroy(gameObject); //duration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
