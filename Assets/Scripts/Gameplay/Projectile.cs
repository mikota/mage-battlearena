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
    float speedFraction = 0.0f;

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
        speedFraction = Mathf.Lerp(speedFraction, 1.0f, Runner.DeltaTime*1.5f);
        transform.localScale = new Vector3(0.75f,0.75f,0.75f) * speedFraction;

        transform.position += direction * speedFraction * speed * Runner.DeltaTime;
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log("Projectile collided with " + collision.gameObject.name);
        /*Health health = collision.gameObject.GetComponent<Health>();
        if (health != null) {
            Debug.Log("HIT for " + damage + " damage!");
            health.TakeDamage(damage);
        }*/
        PlayerNetwork playerNetwork = collision.gameObject.GetComponent<PlayerNetwork>();
        if (playerNetwork != null)
        {
            Debug.Log("HIT for " + damage + " damage!");
            playerNetwork.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

}
