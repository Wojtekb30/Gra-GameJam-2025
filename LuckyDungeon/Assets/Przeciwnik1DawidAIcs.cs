using UnityEngine;

public class Przeciwnik1DawidAIcs : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public bool isActive = false;

    // --------- NOWE: system zdrowia + loot ---------
    public int health = 100;
    public GameObject lootPrefab;
    public bool dropOnDeath = true;

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
        Debug.Log("Wróg aktywowany! Rusza w stronê gracza.");
    }

    // --------- NOWE: otrzymywanie obra¿eñ -----------
    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Die();
    }

    // --------- NOWE: œmieræ + drop ------------------
    void Die()
    {
        if (dropOnDeath && lootPrefab != null)
            Instantiate(lootPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // --------- NOWE: dotkniêcie gracza = 100 dmg ----
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Wróg dotkn¹³ gracza -> otrzymuje 100 dmg.");
            TakeDamage(100);
        }
    }
}
