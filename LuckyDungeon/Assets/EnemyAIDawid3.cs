using UnityEngine;

public class EnemyAIDawid3 : MonoBehaviour
{
    public Transform player;       // pozycja gracza
    public float speed = 3f;       // prêdkoœæ ruchu
    public bool isActive = false;  // czy aktywowany

    // HP + loot
    public int health = 100;
    public GameObject lootPrefab;
    public bool dropOnDeath = true;

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
        Debug.Log("Enemy3: wróg aktywowany! Rusza w stronê gracza.");
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0) Die();
    }

    void Die()
    {
        if (dropOnDeath && lootPrefab != null)
            Instantiate(lootPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy3 OnTriggerEnter z: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy3 dotkn¹³ gracza -> 100 dmg.");
            TakeDamage(100);
        }
    }
}
