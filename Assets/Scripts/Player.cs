using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //// Player components
    public int maxHealth = 100;
    private int _currentHealth;
    public float moveSpeed = 5f;
    public int score = 0;

    // Animation
    public Animator animator;

    //// Projectile components
    public float projectileSpeed = 10f;
    public int projectileDamage = 25;
    public float fireCooldown = 0.75f;

    void Start() {
        _currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void UpdateHealth(int points) {
        _currentHealth += points;

        if (_currentHealth > maxHealth) {
            _currentHealth = maxHealth;
        }
        if (_currentHealth <= 0) {
            Defeat();
        } else if (points < 0) {
            animator.SetTrigger("getHit");
        }
    }

    void Defeat() {
        Debug.Log("Player Defeated!");
        animator.SetTrigger("die");

        GetComponent<CapsuleCollider>().enabled = false;
        //Destroy(gameObject);
    }

    public void UpdateScore(int points) {
        score += points;
    }
}
