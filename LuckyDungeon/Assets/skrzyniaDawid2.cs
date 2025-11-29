using UnityEngine;

public class skrzyniadawid2 : MonoBehaviour
{
    public int minGold1 = 10;
    public int maxGold1 = 20;
    public bool oneTimeOnly = true;

    private bool opened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!opened && other.CompareTag("Player"))
        {
            opened = true;

            int gold = Random.Range(minGold1, maxGold1 + 1); // 10–20
            Debug.Log("Skrzynia: wylosowano " + gold + " sztuk z³ota.");

            // TODO: tu mo¿esz dodaæ z³oto graczowi, np.:
            // other.GetComponent<PlayerGold>()?.AddGold(gold);

            if (!oneTimeOnly)
                opened = false; // jeœli chcesz, ¿eby da³o siê otwieraæ wiele razy
        }
    }
}
