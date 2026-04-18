using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

public class TerminalController : MonoBehaviour, Interactable
{
    [SerializeField] CinemachineCamera monitorZoomCamera;
    [SerializeField] int activeCameraPriority = 20;

    [Header("Логика Игрока и Робота")]
    [SerializeField] Behaviour[] playerComponentsToDisable;
    [SerializeField] Behaviour[] robotComponentsToEnable;

    [Header("События (Звуки, UI, Эффекты)")]
    public UnityEvent OnConnectToRobot;
    public UnityEvent OnDisconnectFromRobot;

    bool _isControllingRobot = false;

    void Start()
    {
        SetComponentsEnabled(robotComponentsToEnable, false);
    }

    public void Interact()
    {
        if (!_isControllingRobot) EnterRobotMode();
    }

    void EnterRobotMode()
    {
        _isControllingRobot = true;
        Bootstrapper.IsPlayerInTerminal = true;
        SetComponentsEnabled(playerComponentsToDisable, false);
        monitorZoomCamera.Priority = activeCameraPriority;
        SetComponentsEnabled(robotComponentsToEnable, true);
        OnConnectToRobot?.Invoke();
        if (InputManager.Instance != null) InputManager.Instance.OnPausePressed += ExitRobotMode;
    }

    void ExitRobotMode()
    {
        _isControllingRobot = false;
        Bootstrapper.IsPlayerInTerminal = false;
        if (InputManager.Instance != null)  InputManager.Instance.OnPausePressed -= ExitRobotMode;
        SetComponentsEnabled(robotComponentsToEnable, false);
        monitorZoomCamera.Priority = 0; 
        SetComponentsEnabled(playerComponentsToDisable, true);
        OnDisconnectFromRobot?.Invoke();
    }

    void SetComponentsEnabled(Behaviour[] components, bool state)
    {
        foreach (var comp in components) if (comp != null) comp.enabled = state;
    }

    private void OnDestroy()
    {
        if (_isControllingRobot && InputManager.Instance != null) InputManager.Instance.OnPausePressed -= ExitRobotMode;
    }
}
