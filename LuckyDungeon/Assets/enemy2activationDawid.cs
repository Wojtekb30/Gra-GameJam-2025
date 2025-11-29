using UnityEngine;

public class enemy2activationDawid : MonoBehaviour
{
    public EnemyAIDawid2 enemy1;   // referencja do wroga ZMIENIÆ
    public Transform player;             // referencja do gracza

    private void OnTriggerEnter(Collider other)
    {
        // sprawdzamy, czy obiekt wchodz¹cy to gracz
        if (other.transform == player)
        {
            enemy1.Activate();
            Debug.Log("Gracz wszed³ w strefê aktywacji wroga.");
        }
    }
}
