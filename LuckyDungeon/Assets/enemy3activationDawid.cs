using UnityEngine;

public class enemy3activationDawid : MonoBehaviour
{
    public EnemyAIDawid3 enemy2;   // referencja do wroga ZMIENIÆ
    public Transform player;             // referencja do gracza

    private void OnTriggerEnter(Collider other)
    {
        // sprawdzamy, czy obiekt wchodz¹cy to gracz
        if (other.transform == player)
        {
            enemy2.Activate();
            Debug.Log("Gracz wszed³ w strefê aktywacji wroga.");
        }
    }
}
