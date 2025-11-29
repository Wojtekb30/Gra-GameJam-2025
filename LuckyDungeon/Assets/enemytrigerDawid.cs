using UnityEngine;

public class enemytrigerDawid : MonoBehaviour
{
    public Przeciwnik1DawidAIcs enemy;   // referencja do wroga
    public Transform player;             // referencja do gracza

    private void OnTriggerEnter(Collider other)
    {
        // sprawdzamy, czy obiekt wchodz¹cy to gracz
        if (other.transform == player)
        {
            enemy.Activate();
            Debug.Log("Gracz wszed³ w strefê aktywacji wroga.");
        }
    }
}
