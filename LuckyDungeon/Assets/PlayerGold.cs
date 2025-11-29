using UnityEngine;
using UnityEngine.UI;

public class PlayerGold : MonoBehaviour
{
    public int gold = 0;
    public Text goldTextUI;

    void Start()
    {
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("Gracz zdoby³ " + amount + " z³ota. Teraz ma: " + gold);
        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        if (goldTextUI != null)
            goldTextUI.text = "Gold: " + gold;
    }
}
