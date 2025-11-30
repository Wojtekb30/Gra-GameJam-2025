using UnityEngine;

public class sklep : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopUI.SetActive(false);
        }
    }
}
