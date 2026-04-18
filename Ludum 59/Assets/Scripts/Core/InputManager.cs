using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.IO;

[Serializable]
public class SettingsSaveData
{
    public string bindings;
    public float mouseSensitivity = 2f;
    public float gamepadSensitivity = 150f;
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerControls InputActions { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsMouseInput { get; private set; }
    public float MouseSensitivity { get; set; } = 2f;
    public float GamepadSensitivity { get; set; } = 150f;
    
    public event Action OnJumpPressed;
    public event Action OnInteractPressed;
    public event Action OnDropPressed;
    public event Action OnPausePressed;
    public event Action OnUnpausePressed;
    public event Action OnSubmitPressed;
    public event Action OnInventoryPressed;
    
    public event Action OnDictaphonePlayUsePressed;
    public event Action OnDictaphonePauseUsePressed;

    public event Action OnLidarScannerPressed;

    public event Action<int> OnHotbarSelected; 

    string bindsSavePath;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
            return;
        }

        InputActions = new PlayerControls();
        bindsSavePath = Path.Combine(Application.persistentDataPath, "keybinds.json");

        LoadBindings();
        InputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        InputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        InputActions.Player.Look.performed += ctx => 
        {
            LookInput = ctx.ReadValue<Vector2>();
            IsMouseInput = ctx.control.device is Mouse;
        };
        InputActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        InputActions.Player.Sprint.performed += ctx => IsSprinting = true;
        InputActions.Player.Sprint.canceled += ctx => IsSprinting = false;

        InputActions.Player.Crouch.performed += ctx => IsCrouching = true;
        InputActions.Player.Crouch.canceled += ctx => IsCrouching = false;

        InputActions.Player.Jump.performed += ctx => IsJumping = true;
        InputActions.Player.Jump.canceled += ctx => IsJumping = false;

        InputActions.Player.Interact.performed += _ => OnInteractPressed?.Invoke();
        InputActions.Player.Drop.performed += _ => OnDropPressed?.Invoke();

        InputActions.Player.LidarScanner.performed += _ => OnLidarScannerPressed?.Invoke();

        InputActions.Player.Pause.performed += _ => OnPausePressed?.Invoke();
        InputActions.Player.Inventory.performed += _ => OnInventoryPressed?.Invoke();
        
        InputActions.UI.UnPause.performed += _ => OnUnpausePressed?.Invoke();
        InputActions.UI.Submit.performed += _ => OnSubmitPressed?.Invoke();
        InputActions.UI.InventoryClose.performed += _ => OnInventoryPressed?.Invoke();

        InputActions.Player.Hotbar1.performed += _ => OnHotbarSelected?.Invoke(0);
        InputActions.Player.Hotbar2.performed += _ => OnHotbarSelected?.Invoke(1);
        InputActions.Player.Hotbar3.performed += _ => OnHotbarSelected?.Invoke(2);
    }
    public void EnablePlayerInput()
    {
        InputActions.UI.Disable();
        InputActions.Player.Enable();
    }

    public void EnableUIInput()
    {
        InputActions.Player.Disable();
        InputActions.UI.Enable();
    }

    void OnEnable() { if (InputActions != null) EnablePlayerInput(); }
    void OnDisable() => InputActions?.Disable();

    public async void SaveBindings()
    {
        SettingsSaveData data = new SettingsSaveData
        {
            bindings = InputActions.SaveBindingOverridesAsJson(),
            mouseSensitivity = MouseSensitivity,
            gamepadSensitivity = GamepadSensitivity
        };
        string json = JsonUtility.ToJson(data, true); 
        await File.WriteAllTextAsync(bindsSavePath, json);
    }

    public void LoadBindings()
    {
        if (File.Exists(bindsSavePath))
        {
            string json = File.ReadAllText(bindsSavePath);
            SettingsSaveData data = JsonUtility.FromJson<SettingsSaveData>(json);

            if (data != null)
            {
                MouseSensitivity = data.mouseSensitivity;
                GamepadSensitivity = data.gamepadSensitivity;
                if (!string.IsNullOrEmpty(data.bindings)) 
                    InputActions.LoadBindingOverridesFromJson(data.bindings);
            }
        }
    }

    public void ResetBindings()
    {
        InputActions.RemoveAllBindingOverrides();
        MouseSensitivity = 2f;
        GamepadSensitivity = 150f;
        SaveBindings();
    }
}