using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;

    public GameObject muzzleFlashPrefab;
    public float muzzleFlashDuration = 0.5f;

    private float lastShotTime;
    private bool isFacingRight = true;

    void Update()
    {
        isFacingRight = transform.localScale.x > 0;

        if (Input.GetKeyDown(KeyCode.Space))
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            flash.transform.SetParent(firePoint);
            Destroy(flash, muzzleFlashDuration);
        }

        Vector2 shootDir = isFacingRight ? Vector2.right : Vector2.left;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = shootDir * bulletSpeed;
        }

        Vector3 bulletScale = bullet.transform.localScale;
        bulletScale.x = Mathf.Abs(bulletScale.x) * (isFacingRight ? 1 : -1);
        bullet.transform.localScale = bulletScale;

        bullet.transform.position = new Vector3(firePoint.position.x, firePoint.position.y, 0);
    }
}
