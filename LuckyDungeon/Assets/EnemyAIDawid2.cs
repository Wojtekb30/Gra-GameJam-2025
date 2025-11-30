using UnityEngine;

public class EnemyAIDawid2 : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public bool isActive = false;

    // --------- NOWE: system zdrowia + loot ---------
    public int health = 100;
    public GameObject lootPrefab;
    public bool dropOnDeath = true;

    public SlotMachineScriptWoj slotMachine;

    // ------------------ AI --------------------------



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
        Debug.Log("Wr�g aktywowany! Rusza w stron� gracza.");
    }

    // --------- NOWE: otrzymywanie obra�e� -----------
    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Die();
    }

    // --------- NOWE: �mier� + drop ------------------
    void Die()
    {
        if (dropOnDeath && lootPrefab != null)
            Instantiate(lootPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // --------- NOWE: dotkni�cie gracza = 100 dmg ----
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Wr�g dotkn�� gracza -> otrzymuje 100 dmg.");
            slotMachine.TriggerSpin();
            TakeDamage(100);
            PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(20);
            }


        }
    }
}
