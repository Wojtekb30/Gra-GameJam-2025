using System;
using System.Collections;
using UnityEngine;

public class Przeciwnik1DawidAIcs : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public bool isActive = false;

    // health + loot
    public int health = 100;
    public GameObject lootPrefab;
    public bool dropOnDeath = true;

    public SlotMachineScriptWoj slotMachine;

    // Drag & drop references for player systems (assign in Inspector)
    [Header("Drag & drop player systems (optional - will fallback to collider lookup)")]
    public PlayerHealth playerHealth;
    public PlayerMana playerMana;
    public PlayerStamina playerStamina;
    public PlayerTime playerTime;

    // Prevent multiple concurrent spins / collisions
    private bool spinInProgress = false;

    void Update()
    {
        if (isActive && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(player);
        }
    }

    public void Activate()
    {
        isActive = true;
        Debug.Log("Wróg aktywowany! Rusza w stronę gracza.");
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (dropOnDeath && lootPrefab != null)
            Instantiate(lootPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || spinInProgress)
            return;

        Debug.Log("Wróg dotknął gracza -> uruchamiam slot machine i czekam na wynik.");

        // Apply immediate contact damage if you want to (kept as example, optional)
        PlayerHealth phImmediate = other.GetComponentInParent<PlayerHealth>();
        if (phImmediate != null)
        {
            phImmediate.TakeDamage(20);
        }

        if (slotMachine == null)
        {
            Debug.LogWarning("No slotMachine assigned!");
            // fallback: immediate heavy damage (if desired)
            TakeDamage(100);
            return;
        }

        // Start coroutine and pass collider so we can fallback to its components if inspector fields are empty
        StartCoroutine(SpinAndThenApplyDamageCoroutine(other));
    }

    private IEnumerator SpinAndThenApplyDamageCoroutine(Collider triggeringCollider)
    {
        spinInProgress = true;

        bool gotResult = false;
        int[] results = null;

        Action<int[]> handler = null;
        handler = (int[] res) =>
        {
            results = res;
            gotResult = true;
        };

        slotMachine.SpinResultEvent += handler;
        slotMachine.TriggerSpin();

        float timeout = 10f;
        float timer = 0f;
        while (!gotResult && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        slotMachine.SpinResultEvent -= handler;

        if (!gotResult)
        {
            Debug.LogWarning("Timed out waiting for slot result. Applying fallback damage.");
            TakeDamage(100);
            spinInProgress = false;
            yield break;
        }

        Debug.Log($"Slot result received: [{string.Join(", ", results)}]. Applying effects now.");

        // Resolve player references (use inspector if assigned, otherwise look up on the collider's parent)
        PlayerHealth ph = playerHealth ?? triggeringCollider.GetComponentInParent<PlayerHealth>();
        PlayerMana pm = playerMana ?? triggeringCollider.GetComponentInParent<PlayerMana>();
        PlayerStamina ps = playerStamina ?? triggeringCollider.GetComponentInParent<PlayerStamina>();
        PlayerTime pt = playerTime ?? triggeringCollider.GetComponentInParent<PlayerTime>();

        if (ph == null || pm == null || ps == null || pt == null)
        {
            Debug.LogError("One or more player system scripts are missing! (PlayerHealth/PlayerMana/PlayerStamina/PlayerTime). Effects will be applied where possible.");
            // we continue but check for null before calling methods
        }

        // --- Interpret slot results --- //
        int totalAttack = 0;
        int totalStaminaCost = 0;
        int totalManaCost = 0;

        int damageMultiplier = 1;
        int staminaMultiplier = 1;

        foreach (var id in results)
        {
            switch (id)
            {
                case 1: // wooden sword - 10 hp attack, costs 5 stamina
                    totalAttack += 10;
                    totalStaminaCost += 5;
                    break;
                case 2: // iron sword - 20 hp attack, costs 10 stamina
                    totalAttack += 20;
                    totalStaminaCost += 10;
                    break;
                case 3: // diamond sword - 40 hp attack, costs 25 stamina and 10 mana
                    totalAttack += 40;
                    totalStaminaCost += 25;
                    totalManaCost += 10;
                    break;
                case 6: // war hammer - 80 hp attack, costs 70 stamina
                    totalAttack += 80;
                    totalStaminaCost += 70;
                    break;

                case 4: // knife - double damage and used stamina
                    damageMultiplier = Math.Max(damageMultiplier, 2);
                    staminaMultiplier = Math.Max(staminaMultiplier, 2);
                    break;
                case 5: // good knife - triple damage and double used stamina
                    damageMultiplier = Math.Max(damageMultiplier, 3);
                    staminaMultiplier = Math.Max(staminaMultiplier, 2);
                    break;

                case 7: // heal potion - add 20 health points (apply immediately)
                    if (ph != null) ph.Heal(20);
                    break;
                case 8: // power potion - add 20 stamina (apply immediately)
                    if (ps != null) ps.RestoreStamina(20);
                    break;
                case 9: // mana potion - add 20 mana (apply immediately)
                    if (pm != null) pm.RestoreMana(20);
                    break;

                case 10: // fire magic - 20 hp attack, use 10 mana
                    totalAttack += 20;
                    totalManaCost += 10;
                    break;
                case 11: // time magic - add 10 units of time by calling HEALTIME
                    if (pt != null) pt.AddTime(10);
                    break;
                case 12: // power magic - 40 hp attack, use 30 mana.
                    totalAttack += 40;
                    totalManaCost += 30;
                    break;
                default:
                    Debug.Log($"Unknown slot id {id}, ignoring.");
                    break;
            }
        }

        // Apply knife stamina multiplier (if any knife was present)
        totalStaminaCost *= staminaMultiplier;

        // --- CHECK RESOURCES BEFORE ATTACK --- //
        int currentStamina = (ps != null) ? ps.currentStamina : int.MaxValue; // if no stamina script -> assume unlimited
        int currentMana = (pm != null) ? pm.currentMana : int.MaxValue;

        bool enoughStamina = currentStamina >= totalStaminaCost;
        bool enoughMana = currentMana >= totalManaCost;

        if (!enoughStamina || !enoughMana)
        {
            Debug.Log("Not enough stamina or mana for attack! Attack canceled.");
            // We applied instant effects (potions/time/heal) earlier; now we skip the attack.
            spinInProgress = false;
            yield break;
        }

        // Subtract used resources
        if (totalStaminaCost > 0 && ps != null)
        {
            ps.UseStamina(totalStaminaCost);
        }

        if (totalManaCost > 0 && pm != null)
        {
            pm.UseMana(totalManaCost);
        }

        // Compute damage and apply to the player
        int damageAmount = totalAttack * damageMultiplier - totalStaminaCost;
        if (damageAmount < 0) damageAmount = 0;

        if (damageAmount > 0 && ph != null)
        {
            TakeDamage(damageAmount);
            Debug.Log($"Applied DAMAGE: {damageAmount}");
        }
        else
        {
            Debug.Log("No damage applied (either zero damage or missing PlayerHealth).");
        }

        spinInProgress = false;
    }

    // Example helper that converts spin results into damage (optional)
    int CalculateDamageFromResults(int[] res)
    {
        if (res == null || res.Length == 0) return 0;
        if (res.Length >= 3 && res[0] == res[1] && res[1] == res[2])
            return 0;
        int sum = 0;
        foreach (var r in res) sum += r;
        return sum;
    }
}
