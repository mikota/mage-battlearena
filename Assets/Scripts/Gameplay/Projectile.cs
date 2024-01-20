using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour {

    //// Projectile components
    [SerializeField] private float duration = 3f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int damage;
    private Rigidbody rigidBody;

    void Awake() {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 initialVelocity) {
        if (rigidBody != null) {
            rigidBody.velocity = initialVelocity * speed;
        }
        Destroy(gameObject, duration);
    }

    void OnCollisionEnter(Collision collision) {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null) {
            Debug.Log("HIT for " + damage + " damage!");
            health.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

}
