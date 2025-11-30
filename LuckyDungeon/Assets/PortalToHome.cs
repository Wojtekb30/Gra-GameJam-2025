using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToHome : MonoBehaviour
{
    public int sceneid= 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Teleportacja do bazy..."+other);
            SceneManager.LoadScene(sceneid); // nazwa sceny
        }
    }
}
