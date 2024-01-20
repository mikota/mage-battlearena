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
    private Vector3 direction;

    void Awake() {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 initialVelocity) {
        if (rigidBody != null) {
            //rigidBody.velocity = initialVelocity * speed;
            direction = initialVelocity.normalized;
        }
        //Destroy(gameObject, duration);
    }

    public override void FixedUpdateNetwork()
    {
        transform.position += direction * speed * Runner.DeltaTime;
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
