using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Range(0.01f, 0.5f)] [SerializeField] float smoothTime; 
    [SerializeField] float minVerticalAngle = -90f;
    [SerializeField] float maxVerticalAngle = 90f;

    const float MOUSE_MULTIPLIER = 0.10f; 
    const float GAMEPAD_MULTIPLIER = 10f;

    float xRotation = 0f;
    float yRotation = 0f;
    float currentXRotation;
    float currentYRotation;
    float xRotationVelocity;
    float yRotationVelocity;
    Transform playerBody;

    void Start()
    {
        playerBody = transform.parent;
        yRotation = playerBody.eulerAngles.y;
        currentYRotation = yRotation;
    }

    void Update()
    {
        if (InputManager.Instance == null) return;

        Vector2 lookInput = InputManager.Instance.LookInput;
        bool isMouse = InputManager.Instance.IsMouseInput;

        float lookX = 0f;
        float lookY = 0f;

        if (isMouse)
        {
            lookX = lookInput.x * InputManager.Instance.MouseSensitivity * MOUSE_MULTIPLIER;
            lookY = lookInput.y * InputManager.Instance.MouseSensitivity * MOUSE_MULTIPLIER;
        }
        else
        {
            lookX = lookInput.x * InputManager.Instance.GamepadSensitivity * GAMEPAD_MULTIPLIER * Time.deltaTime;
            lookY = lookInput.y * InputManager.Instance.GamepadSensitivity * GAMEPAD_MULTIPLIER * Time.deltaTime;
        }

        yRotation += lookX;
        xRotation -= lookY;

        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationVelocity, smoothTime);
        currentYRotation = Mathf.SmoothDampAngle(currentYRotation, yRotation, ref yRotationVelocity, smoothTime);

        transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        playerBody.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }
}