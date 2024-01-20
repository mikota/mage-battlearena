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
    [SerializeField] private float scale = 1.0f;
    [SerializeField] private ParticleSystem particleSystem;
    public PlayerNetwork playerOwner;

    void Awake() {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 initialVelocity, PlayerNetwork pl) {
        if (rigidBody != null) {
            //rigidBody.velocity = initialVelocity * speed;
            direction = initialVelocity.normalized;
        }
        playerOwner = pl;
        //Destroy(gameObject, duration);
    }

    public override void FixedUpdateNetwork()
    {
        speedFraction = Mathf.Lerp(speedFraction, 1.0f, Runner.DeltaTime);
        transform.localScale = new Vector3(0.75f,0.75f,0.75f) * speedFraction * scale;
        //lerp particle system intensity/scale
        var main = particleSystem.main;
        main.startSize = speedFraction * scale;
        main.startSpeed = speedFraction * scale;


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
            if (!playerNetwork.isDead)
            {
                Debug.Log("HIT for " + damage + " damage!");
                if (playerNetwork.TakeDamage(damage)) //killing blow
                {
                    playerOwner.IncKillCount();
                }
            } 
        }
        Destroy(gameObject);
    }

}
