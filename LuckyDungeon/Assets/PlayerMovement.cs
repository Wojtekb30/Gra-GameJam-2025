using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 5f;

    [Header("Skakanie i grawitacja")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Patrzenie (myszka)")]
    public Transform cameraHolder;
    public float mouseSensitivity = 100f;
    public bool invertY = false;
    public float minPitch = -75f;
    public float maxPitch = 75f;

    [Header("Interaction")]
    public SlotMachineScriptWoj slotMachine;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        yaw = transform.eulerAngles.y;

        if (cameraHolder == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraHolder = cam.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();

        // ----------------------------------------
        // Press E to start slot machine
        // ----------------------------------------
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (slotMachine != null)
            {
                //slotMachine.TriggerSpin();
            }

        }

        if (Input.GetKeyDown(KeyCode.F1)) { 
            mouseSensitivity -= 50f; 
            if (mouseSensitivity<50f) { mouseSensitivity = 50f; }
            Debug.Log("Current mouse sensitivity: "+ mouseSensitivity);
            
            }
        if (Input.GetKeyDown(KeyCode.F2)) { 
            mouseSensitivity += 50f; 
            Debug.Log("Current mouse sensitivity: "+ mouseSensitivity);
            }
        if (Input.GetKeyDown(KeyCode.F3)) {
            if (invertY) { invertY = false; } else { invertY = true; } 
             }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch += (invertY ? mouseY : -mouseY);

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
