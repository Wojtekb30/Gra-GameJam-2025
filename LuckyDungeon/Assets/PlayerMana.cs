using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerMana : MonoBehaviour
{
    public int maxMana = 100;
    public int currentMana;
    public TextMeshProUGUI manaTextUI;


    void Start()
    {
        currentMana = maxMana;
        UpdateManaUI();
    }

    public void UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            Debug.Log("Zu�yto " + amount + " many. Pozosta�o: " + currentMana);
        }
        else
        {
            Debug.Log("Za ma�o many!");
        }

        UpdateManaUI();
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;

        if (currentMana > maxMana)
            currentMana = maxMana;

        Debug.Log("Odzyskano " + amount + " many. Aktualnie: " + currentMana);

        UpdateManaUI();
    }

    void UpdateManaUI()
    {
        if (manaTextUI != null)
            manaTextUI.text = "Mana: " + currentMana + "/" + maxMana;
    }
}
