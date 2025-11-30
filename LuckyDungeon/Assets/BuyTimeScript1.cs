using UnityEngine;

/// <summary>
/// When the player is facing this object and presses B, spend all the player's gold
/// to buy the same amount of seconds on the PlayerTime timer, then set gold to 0.
/// 
/// IMPORTANT: Assign the references in the Inspector for the most reliable behavior:
///  - playerTransform: the Transform of your player (or camera/head) used to determine facing
///  - goldScript: reference to the GoldTextScript1 instance
///  - playerTime: reference to the PlayerTime instance
/// 
/// If playerTransform is not assigned, the script will try to find a GameObject tagged "Player".
/// </summary>
public class BuyTimeScript1 : MonoBehaviour
{
    [Header("Interaction (assign in Inspector)")]
    [Tooltip("Assign your Player's Transform (used for position and forward). If null, will try GameObject with tag 'Player'.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Maximum distance (world units) the player can be to interact.")]
    [SerializeField] private float maxInteractDistance = 3f;

    [Tooltip("Maximum facing angle (degrees) between player's forward and direction to this object.")]
    [SerializeField, Range(0f, 90f)] private float maxFacingAngle = 30f;

    [Header("Game references (assign in Inspector)")]
    [Tooltip("Reference to the GoldTextScript1 instance that holds player's gold.")]
    [SerializeField] private GoldTextScript1 goldScript;

    [Tooltip("Reference to the PlayerTime instance that holds the timer.")]
    [SerializeField] private PlayerTime playerTime;

    private void Awake()
    {
        // Try to auto-find player by tag if nothing assigned.
        if (playerTransform == null)
        {
            var playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
                playerTransform = playerGO.transform;
        }

        // Note: It's best to assign goldScript and playerTime in the Inspector.
        // If you haven't assigned them, we try a gentle fallback (FindObjectOfType-like).
        if (goldScript == null)
        {
            // Try to locate any active GoldTextScript1 in scene
            var golds = FindObjectsOfType<GoldTextScript1>();
            if (golds.Length > 0)
                goldScript = golds[0];
        }

        if (playerTime == null)
        {
            var times = FindObjectsOfType<PlayerTime>();
            if (times.Length > 0)
                playerTime = times[0];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // require player reference (to determine facing/distance)
            if (playerTransform == null)
            {
                Debug.LogWarning("BuyTimeScript1: playerTransform is not assigned and no GameObject tagged 'Player' was found.");
                return;
            }

            if (!IsPlayerFacing())
            {
                Debug.Log("BuyTimeScript1: Not facing buy object or out of range.");
                return;
            }

            if (goldScript == null)
            {
                Debug.LogWarning("BuyTimeScript1: goldScript reference not set and none found in scene. Assign it in Inspector.");
                return;
            }

            if (playerTime == null)
            {
                Debug.LogWarning("BuyTimeScript1: playerTime reference not set and none found in scene. Assign it in Inspector.");
                return;
            }

            int gold = goldScript.Gold;
            if (gold <= 0)
            {
                Debug.Log("Brak złota do wydania."); // No gold to spend.
                return;
            }

            int added = playerTime.AddTime(gold);

            // Per requirement: set gold to 0 regardless of how many seconds were actually added
            goldScript.SetGold(0);

            Debug.Log($"Spent {gold} gold to buy {added} seconds. Gold is now 0.");
        }
    }

    private bool IsPlayerFacing()
    {
        Vector3 toTarget = transform.position - playerTransform.position;
        float dist = toTarget.magnitude;
        if (dist > maxInteractDistance) return false;

        Vector3 forward = playerTransform.forward;
        float dot = Vector3.Dot(forward.normalized, toTarget.normalized);
        float requiredDot = Mathf.Cos(maxFacingAngle * Mathf.Deg2Rad);
        return dot >= requiredDot;
    }
}
