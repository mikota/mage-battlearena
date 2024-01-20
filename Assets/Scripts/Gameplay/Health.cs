using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    [SerializeField] private Image healthbar;
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private UnityEvent defeatCallback;
    [SerializeField] private UnityEvent getHitCallback;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // GETTERS
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // SETTERS
    public void SetCurrentHealth(float health)
    {
        currentHealth = health;
        UpdateHealthUI();
    }

    // FUNCTIONS
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
           // if (healthbar != null) healthbar.gameObject.SetActive(false);
           // if (healthText != null) healthText.gameObject.SetActive(false);
           // defeatCallback.Invoke();
        }
        else
            getHitCallback.Invoke();
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthbar == null) return;
        healthbar.fillAmount = currentHealth / maxHealth;
        if (healthText == null) return;
        healthText.text = currentHealth.ToString();
    }
}
