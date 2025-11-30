using UnityEngine;

public class PlayerGold1 : MonoBehaviour
{
    public int gold = 0;
    public PlayerGold playerGold; // referencja do UI

    void Start()
    {
        // aktualizacja UI na starcie
        //playerGold.UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;

        // zaktualizuj UI
        //playerGold.AddGold(amount);
    }
}
