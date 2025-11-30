using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class PlayerStamina : MonoBehaviour
{
    public int maxStamina = 100;
    public int currentStamina;
    public TextMeshProUGUI staminaTextUI;



    void Start()
    {
        currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    public void UseStamina(int amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            Debug.Log("Zu�yto " + amount + " staminy. Pozosta�o: " + currentStamina);
        }
        else
        {
            Debug.Log("Za ma�o staminy!");
        }

        UpdateStaminaUI();
    }

    public void RestoreStamina(int amount)
    {
        currentStamina += amount;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        Debug.Log("Odzyskano " + amount + " staminy. Aktualnie: " + currentStamina);

        UpdateStaminaUI();
    }

    void UpdateStaminaUI()
    {
        if (staminaTextUI != null)
            staminaTextUI.text = "Stamina: " + currentStamina + "/" + maxStamina;
    }
}
