using UnityEngine;

public class EnemyAIDawid3 : MonoBehaviour
{
    public Transform player;       // pozycja gracza
    public float speed = 3f;       // prêdkoœæ ruchu
    public bool isActive = false;  // czy aktywowany

    void Update()
    {
        if (isActive && player != null)
        {
            // kierunek do gracza
            Vector3 direction = (player.position - transform.position).normalized;

            // ruch w stronê gracza
            transform.position += direction * speed * Time.deltaTime;

            // obrót w stronê gracza (opcjonalne)
            transform.LookAt(player);
        }
    }

    // ta metoda zostanie wywo³ana z triggera ActivationZone
    public void Activate()
    {
        isActive = true;
        Debug.Log("Wróg aktywowany! Rusza w stronê gracza.");
    }
}