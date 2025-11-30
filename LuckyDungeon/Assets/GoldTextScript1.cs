using UnityEngine;
using TMPro;

public class GoldTextScript1 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldTextUI;   // assign in Inspector
    [SerializeField] private int startGold = 0;            // initial amount if not set yet

    // Shared gold stored statically so it survives scene changes as long as the app runs.
    private static int sharedGold = int.MinValue; // sentinel to detect first initialization
    public int Gold => sharedGold;

    private void Awake()
    {
        // Initialize sharedGold on first creation
        if (sharedGold == int.MinValue)
            sharedGold = Mathf.Max(0, startGold);
    }

    private void OnEnable()
    {
        // Update UI when this instance becomes active
        UpdateGoldUI();
    }

    /// <summary>
    /// Adds gold. Returns true if a positive amount was added.
    /// </summary>
    public bool AddGold(int amount)
    {
        if (amount <= 0) return false;
        sharedGold = Mathf.Max(0, sharedGold + amount);
        Debug.Log($"Gracz zdobył {amount} złota. Teraz ma: {sharedGold}");
        UpdateGoldUI();
        return true;
    }

    /// <summary>
    /// Spends gold. Returns true only if enough gold existed.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;
        if (sharedGold >= amount)
        {
            sharedGold -= amount;
            Debug.Log($"Wydano {amount} złota. Pozostało: {sharedGold}");
            UpdateGoldUI();
            return true;
        }
        Debug.Log("Za mało złota!");
        return false;
    }

    /// <summary>
    /// Set gold explicitly (useful for load/save).
    /// </summary>
    public void SetGold(int amount)
    {
        sharedGold = Mathf.Max(0, amount);
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldTextUI == null) return;
        goldTextUI.text = $"Gold: {sharedGold}";
    }

    // Optional: expose a static helper for other scripts that don't have a reference to an instance
    public static void AddGoldStatic(int amount) => sharedGold = Mathf.Max(0, sharedGold + amount);
    public static bool SpendGoldStatic(int amount)
    {
        if (amount <= 0 || sharedGold < amount) return false;
        sharedGold -= amount;
        return true;
    }
}
