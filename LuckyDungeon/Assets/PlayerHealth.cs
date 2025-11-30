using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public TextMeshProUGUI healthTextUI;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Gracz otrzyma� " + dmg + " obra�e�. Aktualne zdrowie: " + currentHealth);

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
        Debug.Log("Gracz umar�!");
        // TODO: tu mo�esz da� respawn, ekran �mierci itp.
    }

    void UpdateHealthUI()
    {
        if (healthTextUI != null)
            healthTextUI.text = "HP: " + currentHealth + "/" + maxHealth;
    }
}
