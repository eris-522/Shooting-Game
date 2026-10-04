using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Feature: Bullet lifespan and damage metrics
    public float lifeTime = 3f;
    public float damage = 15f;

    void Start()
    {
        // Cleanup bullet object after lifeTime seconds
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object contains a DestructibleTarget component
        DestructibleTarget target = collision.gameObject.GetComponent<DestructibleTarget>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // Apply impulse if target has a dynamic Rigidbody
        Rigidbody targetRb = collision.gameObject.GetComponent<Rigidbody>();
        if (targetRb != null && !targetRb.isKinematic)
        {
            targetRb.AddForce(-collision.contacts[0].normal * 300f);
        }

        // Destroy the bullet instance on impact
        Destroy(gameObject);
    }
}