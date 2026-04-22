using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField, Range(0.01f, 0.5f)] float inputSmoothTime = 0.1f; 

    [Header("Настройки приседания")]
    [SerializeField] float standingHeight = 2f;
    [SerializeField] float crouchingHeight = 1f;
    [SerializeField] float crouchSpeed = 10f;
    [SerializeField] Transform cameraTransform;
    [SerializeField] LayerMask obstacleMask; 
    [SerializeField] float checkRadius = 0.4f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] float stepDistance = 1.5f;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;
    bool wasGrounded;
    bool isCrouching;
    float currentHeight;
    float stepCycle;

    Vector2 currentMoveInput; 
    Vector2 moveInputVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentHeight = standingHeight;
        Bootstrapper.PlayerTransform = transform;
    }

    void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;
        if (!wasGrounded && isGrounded) HandleLanding();
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        HandleCrouchLogic();
        ApplyCrouchLerp();
        HandleMovement();
        HandleJumpLogic();
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector2 input = InputManager.Instance != null ? InputManager.Instance.MoveInput : Vector2.zero;
        bool isSprinting = InputManager.Instance != null && InputManager.Instance.IsSprinting;
        currentMoveInput = Vector2.SmoothDamp(currentMoveInput, input, ref moveInputVelocity, inputSmoothTime);
        float currentSpeed = isCrouching ? walkSpeed * 0.5f : (isSprinting ? runSpeed : walkSpeed);
        Vector3 move = transform.right * currentMoveInput.x + transform.forward * currentMoveInput.y;
        if (move.magnitude > 1) move.Normalize();
        Vector3 motion = move * currentSpeed * Time.deltaTime;
        controller.Move(motion);
        CalculateFootsteps(motion.magnitude);
    }

    void CalculateFootsteps(float distanceMoved)
    {
        if (!isGrounded || distanceMoved < 0.001f) return;
        stepCycle += distanceMoved;
        if (stepCycle >= stepDistance)
        {
            PlayFootstepSound();
            stepCycle = 0f;
        }
    }
    void PlayFootstepSound()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;
        int randomIndex = Random.Range(0, footstepSounds.Length);
        audioSource.PlayOneShot(footstepSounds[randomIndex]);
    }

    void HandleJumpLogic()
    {
        bool isJumpPressed = InputManager.Instance != null && InputManager.Instance.IsJumping;
        if (isJumpPressed && isGrounded && !isCrouching && CanStandUp())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            audioSource.PlayOneShot(jumpSound);
        }
    }

    void HandleLanding()
    {
        audioSource.PlayOneShot(landSound);
        stepCycle = stepDistance / 2f;
    }

    void HandleCrouchLogic()
    {
        bool isCrouchPressed = InputManager.Instance != null && InputManager.Instance.IsCrouching;
        if (isCrouchPressed) isCrouching = true;
        else if (CanStandUp()) isCrouching = false;
    }

    void ApplyCrouchLerp()
    {
        float targetH = isCrouching ? crouchingHeight : standingHeight;
        currentHeight = Mathf.Lerp(currentHeight, targetH, Time.deltaTime * crouchSpeed);
        float lastHeight = controller.height;
        controller.height = currentHeight;
        
        controller.center = new Vector3(0, controller.height / 2f, 0);
        transform.position += new Vector3(0, (controller.height - lastHeight) / 2f, 0);

        float camY = isCrouching ? crouchingHeight * 0.8f : standingHeight * 0.8f;
        Vector3 newCamPos = cameraTransform.localPosition;
        newCamPos.y = Mathf.Lerp(newCamPos.y, camY, Time.deltaTime * crouchSpeed);
        cameraTransform.localPosition = newCamPos;
    }

    bool CanStandUp()
    {
        Vector3 startPoint = transform.position + Vector3.up * (currentHeight - checkRadius);
        float distance = standingHeight - currentHeight;
        if (distance <= 0.05f) return true;
        return !Physics.SphereCast(startPoint, checkRadius, Vector3.up, out _, distance, obstacleMask);
    }

    void OnDestroy()
    {
        if (Bootstrapper.PlayerTransform == transform) Bootstrapper.PlayerTransform = null;
    }
}