using UnityEngine;

public class SimpleGun : MonoBehaviour
{
    public float range = 100f;          // zasiêg strza³u
    public Camera playerCamera;         // kamera gracza

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // LPM
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        // promieñ lec¹cy do przodu od kamery
        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out hit,
                            range))
        {
            Debug.Log("Trafiono: " + hit.transform.name);
        }
        else
        {
            Debug.Log("Nie trafiono w nic.");
        }
    }
}
