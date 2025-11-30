using UnityEngine;
using TMPro;   // TextMeshPro namespace

public class PlayerGold : MonoBehaviour
{
    private TextMeshProUGUI goldTextUI;   // assign in Inspector
    [SerializeField] private int startGold = 0;            // optional initial amount

    private int gold;
    public int Gold => gold;                               // read‑only for other scripts

    void Start()
    {
        gold = startGold;
        UpdateGoldUI();
    }

    /// <summary>
    /// Adds gold. Returns true if a positive amount was added.
    /// </summary>
    public bool AddGold(int amount)
    {
        if (amount <= 0) return false;

        gold += amount;
        Debug.Log($"Gracz zdobył {amount} złota. Teraz ma: {gold}");
        UpdateGoldUI();
        return true;
    }

    /// <summary>
    /// Spends gold (e.g., for purchases). Returns true only if enough gold existed.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;

        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log($"Wydano {amount} złota. Pozostało: {gold}");
            UpdateGoldUI();
            return true;
        }

        Debug.Log("Za mało złota!");
        return false;
    }

    private void UpdateGoldUI()
    {
        if (goldTextUI == null) return;
        goldTextUI.text = $"Gold: {gold}";
    }
}
