using UnityEngine;

// Wymaga, żeby na obiekcie być CharacterController
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 5f;      // prędkość chodzenia

    [Header("Skakanie i grawitacja")]
    public float gravity = -9.81f;    // grawitacja (musi być ujemna)
    public float jumpHeight = 1.5f;   // wysokość skoku

    [Header("Patrzenie (myszka)")]
    public Transform cameraHolder;    // przypisz obiekt trzymający kamerę (dziecko Player)
    public float mouseSensitivity = 100f;
    public bool invertY = false;
    public float minPitch = -75f;
    public float maxPitch = 75f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float pitch = 0f; // obrót kamery wokół X
    private float yaw = 0f;   // obrót gracza wokół Y

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // ustaw początkowy yaw na obecną rotację obiektu
        yaw = transform.eulerAngles.y;

        // jeśli cameraHolder nie jest ustawiony, spróbuj znaleźć kamerę w dzieciach
        if (cameraHolder == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraHolder = cam.transform;
        }

        // schowaj i zablokuj kursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch += (invertY ? mouseY : -mouseY);

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Obrót gracza (yaw)
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Obrót kamery (pitch) względem cameraHolder lokalnie
        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Opcjonalnie: odblokowanie kursora po naciśnięciu Esc
    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
