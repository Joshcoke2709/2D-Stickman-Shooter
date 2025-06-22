using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f; //forced cooldown

    public GameObject muzzleFlashPrefab;
    public float muzzleFlashDuration = 0.5f; //flash duration

    private float lastShotTime;
    private bool isFacingRIght = true;

    void Update()
    {
        isFacingRIght = transform.localScale.x > 0;
        //firePoint.localPosition = isFacingRIght ? new Vector2(0.9f, -0.48f) : new Vector2(-0.9f, -0.48f);
        if (Input.GetMouseButtonDown(0))
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
        GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation); // <--NEW (Muzzle flash now rotated to match firePoint)
        flash.transform.SetParent(firePoint); // <--NEW (Flash follows gun tip)
        Destroy(flash, muzzleFlashDuration);

        // Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;
        //bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        Vector2 shootDir = isFacingRIght ? Vector2.right : Vector2.left;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = shootDir * bulletSpeed;

        if (!isFacingRIght)
        {
            Vector3 bulletScale = bullet.transform.localScale;
            bulletScale.x *= -1;
            bullet.transform.localScale = bulletScale;
        }

        /*
        Vector2 shootDir = isFacingRIght ? Vector2.right : Vector2.left; //Direction logic (if isfacingRight = true, then Vector2.right is true, else, Vector2.left.
        bullet.GetComponent<Rigidbody2D>().velocity = shootDir * bulletSpeed; //applies direction for when the player if facing left*/

    }
}
