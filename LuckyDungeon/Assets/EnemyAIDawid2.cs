using UnityEngine;

public class EnemyAIDawid2 : MonoBehaviour
{
    public Transform player;       // pozycja gracza
    public float speed = 3f;       // prêdkoœæ ruchu
    public bool isActive = false;  // czy aktywowany

    // --- NOWE: zdrowie + loot ---
    public int health = 100;
    public GameObject lootPrefab;
    public bool dropOnDeath = true;

    void Update()
    {
        if (isActive && player != null)
        {
            // kierunek do gracza
            Vector3 direction = (player.position - transform.position).normalized;

            // ruch w stronê gracza
            transform.position += direction * speed * Time.deltaTime;

            // obrót w stronê gracza
            transform.LookAt(player);
        }
    }

    // wywo³ywane z triggera ActivationZone
    public void Activate()
    {
        isActive = true;
        Debug.Log("Enemy2: wróg aktywowany! Rusza w stronê gracza.");
    }

    // --- obra¿enia ---
    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Die();
    }

    // --- œmieræ + drop ---
    void Die()
    {
        if (dropOnDeath && lootPrefab != null)
            Instantiate(lootPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // --- dotkniêcie gracza = 100 dmg ---
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy2 OnTriggerEnter z: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy2 dotkn¹³ gracza -> otrzymuje 100 dmg.");
            TakeDamage(100);
        }
    }
}
