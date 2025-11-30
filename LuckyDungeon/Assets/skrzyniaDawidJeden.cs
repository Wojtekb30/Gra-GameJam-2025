using UnityEngine;

public class ChestGold : MonoBehaviour
{
    [Header("Gold range")]
    [SerializeField] private int minGold = 10;
    [SerializeField] private int maxGold = 20;

    [Header("Chest settings")]
    [SerializeField] private bool oneTimeOnly = true;

    // Assign the player’s GoldTextScript1 component in the Inspector
    public GoldTextScript1 goldScript;   // can also be set at runtime

    private bool opened = false;

    private void OnTriggerEnter(Collider other)
    {
        // Ensure we only react to the player and that the chest isn’t already opened
        if (opened || !other.CompareTag("Player")) return;

        opened = true;

        int amount = Random.Range(minGold, maxGold + 1);

        if (goldScript != null)
        {
            goldScript.AddGold(amount);
        }
        else
        {
            Debug.LogWarning("ChestGold: GoldTextScript1 reference not set.");
        }

        // Allow re‑opening if the chest is not one‑time only
        if (!oneTimeOnly)
        {
            opened = false;
        }
    }
}
