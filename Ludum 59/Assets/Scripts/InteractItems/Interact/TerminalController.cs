using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;
using DG.Tweening;

public class TerminalController : MonoBehaviour, Interactable
{
    [SerializeField] AudioSource enterTheRobot;
    [SerializeField] AudioSource exitTheRobot;
    [SerializeField] GameObject ambientPlayer;
    [SerializeField] GameObject soudnsSubmarine;
    [SerializeField] GameObject ambientRobot;
    [SerializeField] CinemachineCamera monitorZoomCamera;
    [SerializeField] int activeCameraPriority = 20;
    [SerializeField] CanvasGroup playerUIGroup;
    [SerializeField] CanvasGroup robotUIGroup;
    [SerializeField] float uiFadeDuration = 0.5f;
    [SerializeField] Ease uiEase = Ease.OutSine;
    [SerializeField] Behaviour[] playerComponentsToDisable;
    [SerializeField] Behaviour[] robotComponentsToEnable;
    public UnityEvent OnConnectToRobot;
    public UnityEvent OnDisconnectFromRobot;

    bool _isControllingRobot = false;

    void Start()
    {
        SetComponentsEnabled(robotComponentsToEnable, false);
        if (robotUIGroup != null)
        {
            robotUIGroup.alpha = 0f;
            robotUIGroup.interactable = false;
            robotUIGroup.blocksRaycasts = false;
        }
    }

    public void Interact()
    {
        if (!_isControllingRobot) EnterRobotMode();
    }

    void EnterRobotMode()
    {
        enterTheRobot.Play();
        _isControllingRobot = true;
        Bootstrapper.IsPlayerInTerminal = true;
        SetComponentsEnabled(playerComponentsToDisable, false);
        monitorZoomCamera.Priority = activeCameraPriority;
        SetComponentsEnabled(robotComponentsToEnable, true);
        FadeUI(playerUIGroup, false);
        FadeUI(robotUIGroup, true);
        ambientPlayer.SetActive(false);
        soudnsSubmarine.SetActive(false);
        ambientRobot.SetActive(true);
        OnConnectToRobot?.Invoke();
        if (InputManager.Instance != null) InputManager.Instance.OnPausePressed += ExitRobotMode;
    }

    void ExitRobotMode()
    {
        exitTheRobot.Play();
        _isControllingRobot = false;
        Bootstrapper.IsPlayerInTerminal = false;
        if (InputManager.Instance != null)  InputManager.Instance.OnPausePressed -= ExitRobotMode;
        SetComponentsEnabled(robotComponentsToEnable, false);
        monitorZoomCamera.Priority = 0; 
        SetComponentsEnabled(playerComponentsToDisable, true);
        FadeUI(robotUIGroup, false);
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
            group.DOFade(1f, uiFadeDuration)
                .SetUpdate(true)
                .SetEase(uiEase); 
                
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.DOFade(0f, uiFadeDuration)
                .SetUpdate(true)
                .SetEase(uiEase);
                
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    void SetComponentsEnabled(Behaviour[] components, bool state)
    {
        foreach (var comp in components) if (comp != null) comp.enabled = state;
    }

    private void OnDestroy()
    {
        if (_isControllingRobot && InputManager.Instance != null) InputManager.Instance.OnPausePressed -= ExitRobotMode;
        if (playerUIGroup != null) playerUIGroup.DOKill();
        if (robotUIGroup != null) robotUIGroup.DOKill();
    }
}
