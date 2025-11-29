using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Text healthTextUI;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Gracz otrzyma³ " + dmg + " obra¿eñ. Aktualne zdrowie: " + currentHealth);

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
        Debug.Log("Gracz umar³!");
        // TODO: tu mo¿esz daæ respawn, ekran œmierci itp.
    }

    void UpdateHealthUI()
    {
        if (healthTextUI != null)
            healthTextUI.text = "HP: " + currentHealth + "/" + maxHealth;
    }
}
