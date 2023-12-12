using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    //// Projectile components
    private Rigidbody _rigidBody;
    public float projectileDuration = 3f;
    private int _damage;

    void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        if (_rigidBody == null) {
            Debug.LogError("Projectile is missing a RigidBody component.");
        }
        Invoke("DestroyProjectile", projectileDuration);
    }

    public void Initialize(Vector3 initialVelocity, int damage) {
        if (_rigidBody != null) {
            _rigidBody.velocity = initialVelocity;
        }
        _damage = damage;
    }

    void OnCollisionEnter(Collision collision) {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null) {
            Debug.Log("HIT for " + _damage + " damage!");
            player.UpdateHealth(-_damage);
        }
        DestroyProjectile();
    }

    void DestroyProjectile() {
        Destroy(gameObject);
    }
}
