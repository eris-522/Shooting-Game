using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    // Feature: Weapon firing parameters
    [Header("Firing Settings")]
    public float bulletSpeed = 70f;
    public float fireRate = 0.15f;
    public float maxRange = 500f;

    // Feature: Object and spawn references
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera fpsCamera;

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (Mouse.current == null) return;

        // Left-click detection
        if (Mouse.current.leftButton.isPressed && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            ShootBullet();
        }
    }

    // Functionality: Projects a ray from the camera center to find the exact target point, then propels the bullet toward it
    void ShootBullet()
    {
        if (bulletPrefab == null || firePoint == null || fpsCamera == null) return;

        // 1. Raycast from the center of the screen (crosshair)
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, maxRange))
        {
            // Point on the surface hit by crosshair
            targetPoint = hit.point;
        }
        else
        {
            // Point in mid-air at maximum range if nothing was hit
            targetPoint = ray.GetPoint(maxRange);
        }

        // 2. Calculate the normalized launch vector from barrel to target point
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        // 3. Spawn bullet aligned with launch direction
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Reset lingering velocities and propel toward target point
            rb.linearVelocity = shootDirection * bulletSpeed;
        }
    }
}