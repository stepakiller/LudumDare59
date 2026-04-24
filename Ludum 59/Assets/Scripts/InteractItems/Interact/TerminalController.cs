using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;
using DG.Tweening;

public class TerminalController : MonoBehaviour, Interactable
{
    [SerializeField] RandomPunches randomPunches;
    [SerializeField] AudioSource enterTheRobot;
    [SerializeField] AudioSource exitTheRobot;
    [SerializeField] GameObject ambientPlayer;
    [SerializeField] GameObject soudnsSubmarine;
    [SerializeField] GameObject ambientRobot;
    [SerializeField] CinemachineCamera monitorZoomCamera;
    [SerializeField] int activeCameraPriority = 20;
    [SerializeField] CanvasGroup playerUIGroup;
    [SerializeField] float uiFadeDuration = 0.5f;
    [SerializeField] Ease uiEase = Ease.OutSine;
    [SerializeField] Behaviour[] playerComponentsToDisable;
    [SerializeField] Behaviour[] robotComponentsToEnable;
    [SerializeField] GameObject[] slides;
    [SerializeField] Collider _collider;
    public UnityEvent OnConnectToRobot;
    public UnityEvent OnDisconnectFromRobot;
    bool _isControllingRobot = false;
    void Start()
    {
        SetComponentsEnabled(robotComponentsToEnable, false);
        EnterRobotMode();
    }

    public void Interact()
    {
        if (!_isControllingRobot) EnterRobotMode();
    }

    void EnterRobotMode()
    {
        if (slides[0].activeInHierarchy || slides[1].activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        enterTheRobot.Play();
        _isControllingRobot = true;
        Bootstrapper.IsPlayerInTerminal = true;
        SetComponentsEnabled(playerComponentsToDisable, false);
        monitorZoomCamera.Priority = activeCameraPriority;
        SetComponentsEnabled(robotComponentsToEnable, true);
        FadeUI(playerUIGroup, false);
        ambientPlayer.SetActive(false);
        soudnsSubmarine.SetActive(false);
        ambientRobot.SetActive(true);
        OnConnectToRobot?.Invoke();
        if (InputManager.Instance != null) InputManager.Instance.OnPausePressed += ExitRobotMode;
    }

    public void ExitRobotMode()
    {
        if (slides[0].activeInHierarchy || slides[1].activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        exitTheRobot.Play();
        _isControllingRobot = false;
        Bootstrapper.IsPlayerInTerminal = false;
        if (InputManager.Instance != null)  InputManager.Instance.OnPausePressed -= ExitRobotMode;
        SetComponentsEnabled(robotComponentsToEnable, false);
        monitorZoomCamera.Priority = 0; 
        SetComponentsEnabled(playerComponentsToDisable, true);
        FadeUI(playerUIGroup, true);
        ambientPlayer.SetActive(true);
        soudnsSubmarine.SetActive(true);
        ambientRobot.SetActive(false);
        OnDisconnectFromRobot?.Invoke();
    }
    void FadeUI(CanvasGroup group, bool show)
    {
        if (group == null) return;
        group.DOKill(); 
        if (show)
        {
            group.DOFade(1f, uiFadeDuration).SetUpdate(true).SetEase(uiEase); 
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.DOFade(0f, uiFadeDuration).SetUpdate(true).SetEase(uiEase);
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    void SetComponentsEnabled(Behaviour[] components, bool state)
    {
        foreach (var comp in components) if (comp != null) comp.enabled = state;
    }

    void OnDestroy()
    {
        if (_isControllingRobot && InputManager.Instance != null) InputManager.Instance.OnPausePressed -= ExitRobotMode;
        if (playerUIGroup != null) playerUIGroup.DOKill();
    }
}
