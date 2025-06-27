using UnityEngine;

public class Player2Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;             // Bullet prefab to instantiate
    public Transform firePoint;                 // Where the bullet spawns from
    public float bulletSpeed = 10f;             // Speed of bullet
    public float fireRate = 0.5f;               // Cooldown between shots

    public GameObject muzzleFlashPrefab;        // Muzzle flash prefab
    public float muzzleFlashDuration = 0.5f;    // Duration of flash

    private float lastShotTime;
    private bool isFacingRight = false;         // Starts facing LEFT since Player 2 is on the right

    void Update()
    {
        isFacingRight = transform.localScale.x > 0;

        if (Input.GetMouseButtonDown(0)) {
        {
            if (Time.time - lastShotTime >= fireRate)
            {
                Shoot();
                lastShotTime = Time.time;
            }
        }
    }

    void Shoot()
    {
        // Create bullet and flash at firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
        flash.transform.SetParent(firePoint);
        Destroy(flash, muzzleFlashDuration);

        // Set direction based on facing
        Vector2 shootDir = isFacingRight ? Vector2.right : Vector2.left;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Prevent falling
        rb.velocity = shootDir * bulletSpeed;

        // Flip bullet sprite visually
        Vector3 bulletScale = bullet.transform.localScale;
        bulletScale.x = Mathf.Abs(bulletScale.x) * (isFacingRight ? 1 : -1);
        bullet.transform.localScale = bulletScale;

        // Ensure bullet is on correct Z layer (optional)
        bullet.transform.position = new Vector3(firePoint.position.x, firePoint.position.y, 0);

        // Auto-destroy bullet after time
        Destroy(bullet, 3f);
    }
}
