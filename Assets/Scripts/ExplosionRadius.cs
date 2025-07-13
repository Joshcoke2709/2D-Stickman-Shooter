using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    public float damageRadius = 1.5f;

    private void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);

        foreach (var hit in hits)
        {
            PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if(health != null)
            {
                // Apply damage to the player
                health.RegisterHit(); // Assuming TakeDamage is a method in PlayerHealth that handles damage
            }
        }
        Destroy(gameObject, 0.5f); // Destroy the explosion radius object after applying damage
    }

}
 