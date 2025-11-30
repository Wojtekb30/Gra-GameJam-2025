using UnityEngine;
using UnityEngine.SceneManagement;

public class portaldodungeonu : MonoBehaviour
{

    public string sceneToLoad = "David";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // wa¿ne: tylko gracz aktywuje
        {
            Debug.Log("Teleportacja do sceny: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
