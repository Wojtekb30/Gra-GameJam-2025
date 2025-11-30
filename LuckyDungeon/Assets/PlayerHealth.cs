using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public TextMeshProUGUI healthTextUI;

    // Event to notify listeners that the player died
    public event Action OnDied;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Gracz otrzymał " + dmg + " obrażeń. Aktualne zdrowie: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    void Die()
    {
        Debug.Log("Gracz umarł!");
        OnDied?.Invoke();
        // TODO: respawn, death screen etc. GameUIController will show the lose panel if present.
    }

    void UpdateHealthUI()
    {
        if (healthTextUI != null)
            healthTextUI.text = "HP: " + currentHealth + "/" + maxHealth;
    }
}
