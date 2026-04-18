using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    [Header("Позиция")]
    [SerializeField] private float walkingBobbingSpeed = 14f;
    [SerializeField] private float bobbingAmount = 0.025f;
    [SerializeField] private float runningBobbingFactor = 1.5f;
    [SerializeField] private float transitionSpeed = 10f; 

    [Header("Вращение")]
    [SerializeField] private float bobbingRotX = 0.3f;
    [SerializeField] private float bobbingRotY = 0.25f;
    [SerializeField] private float runningRotFactor = 1.5f; 

    [Header("Настройки приземления")]
    [SerializeField] private float landDipAmount = 0.4f;   
    [SerializeField] private float landEntrySpeed = 15f;   
    [SerializeField] private float landReturnSpeed = 5f;   

    [Header("Ссылки")]
    [SerializeField] private CharacterController controller;

    private float defaultPosY = 0;
    private float timer = 0;
    private Vector3 targetPos;
    private Quaternion targetRot; 
    private float currentLandOffset = 0f;
    private float targetLandOffset = 0f; 
    private bool wasGrounded;
    private Vector3 lastPosition;
    private bool wasTimeFrozen;

    private void Start()
    {
        defaultPosY = transform.localPosition.y;
        targetPos = transform.localPosition;
        targetRot = transform.localRotation;
        wasGrounded = true;
        
        if (controller != null)
            lastPosition = controller.transform.position;
    }

    private void LateUpdate()
    {
        if (controller == null) return;

        float dt = Time.deltaTime;
        if (Time.timeScale <= 0f || dt <= 0f)
        {
            wasTimeFrozen = true;
            return;
        }

        if (wasTimeFrozen)
        {
            wasTimeFrozen = false;
            wasGrounded = controller.isGrounded;
            targetLandOffset = 0f;
            currentLandOffset = 0f;
            lastPosition = controller.transform.position;
            HandleBobbing(dt);
            return;
        }

        HandleLanding(dt);
        HandleBobbing(dt);
    }

    private void HandleLanding(float dt)
    {
        if (!wasGrounded && controller.isGrounded && dt > 1e-5f) 
            targetLandOffset = landDipAmount;
            
        wasGrounded = controller.isGrounded;

        if (targetLandOffset > 0)
        {
            currentLandOffset = Mathf.Lerp(currentLandOffset, targetLandOffset, dt * landEntrySpeed);
            if (Mathf.Abs(currentLandOffset - targetLandOffset) < 0.05f) 
                targetLandOffset = 0;
        }
        else 
        {
            currentLandOffset = Mathf.Lerp(currentLandOffset, 0, dt * landReturnSpeed);
        }
    }

    private void HandleBobbing(float dt)
    {
        if (dt <= 0f) return;

        float targetPosX = 0;
        float targetPosY = defaultPosY;
        float targetRotX = 0;
        float targetRotY = 0;

        Vector3 currentPos = controller.transform.position;
        Vector3 horizontalPos = new Vector3(currentPos.x, 0, currentPos.z);
        Vector3 horizontalLastPos = new Vector3(lastPosition.x, 0, lastPosition.z);

        float speed = Vector3.Distance(horizontalPos, horizontalLastPos) / dt;
        lastPosition = currentPos;

        if (speed > 0.1f && controller.isGrounded)
        {
            float waveSpeed = walkingBobbingSpeed;
            float waveAmount = bobbingAmount;
            float rotMultX = bobbingRotX;
            float rotMultY = bobbingRotY;

            if (InputManager.Instance != null && InputManager.Instance.IsSprinting)
            {
                waveSpeed *= 1.3f;
                waveAmount *= runningBobbingFactor;
                rotMultX *= runningRotFactor;
                rotMultY *= runningRotFactor;
            }
            
            timer += dt * waveSpeed;
            targetPosX = Mathf.Cos(timer / 2) * waveAmount;
            targetPosY = defaultPosY + Mathf.Sin(timer) * waveAmount;
            targetRotX = Mathf.Sin(timer) * rotMultX;
            targetRotY = Mathf.Cos(timer / 2) * rotMultY;
        }
        else
        {
            timer = 0;
            targetPosX = 0;
            targetPosY = defaultPosY;
            targetRotX = 0;
            targetRotY = 0;
        }
        
        Vector3 bobbingPos = new Vector3(targetPosX, targetPosY, 0);
        targetPos = Vector3.Lerp(targetPos, bobbingPos, dt * transitionSpeed);
        transform.localPosition = new Vector3(targetPos.x, targetPos.y - currentLandOffset, targetPos.z);
        
        Quaternion bobbingRot = Quaternion.Euler(targetRotX, targetRotY, 0);
        targetRot = Quaternion.Slerp(targetRot, bobbingRot, dt * transitionSpeed);
        transform.localRotation = targetRot;
    }
}