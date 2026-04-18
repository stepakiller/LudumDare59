using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; 
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class RebindButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bindButtonText;  
    [SerializeField] Button rebindButton;
    [SerializeField] string actionMapName = "Player"; 
    [SerializeField] string actionName = "Jump";      
    [SerializeField] int bindingIndex = 0; 
    [SerializeField] string controlScheme = "KeyboardMouse";
    [SerializeField] string[] secondaryActions; 

    InputAction actionToRebind;
    InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    public static event Action OnAnyBindingChanged;

    void Start()
    {
        actionToRebind = InputManager.Instance.InputActions.asset.FindAction($"{actionMapName}/{actionName}");
        if (actionToRebind == null) return;
        UpdateUI(); 
        rebindButton.onClick.AddListener(StartRebinding);
    }

    void OnEnable() => OnAnyBindingChanged += UpdateUI;
    void OnDisable() => OnAnyBindingChanged -= UpdateUI;

    void StartRebinding()
    {
        rebindButton.interactable = false;
        bindButtonText.text = "...";

        InputManager.Instance.InputActions.Disable();

        rebindingOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithBindingGroup(controlScheme)
            .WithControlsExcluding("Mouse/position") 
            .WithControlsExcluding("Mouse/delta")
            .WithCancelingThrough("<Keyboard>/escape") 
            .OnComplete(operation => RebindComplete()) 
            .OnCancel(operation => RebindCancelled()) 
            .Start();
    }

    void RebindComplete()
    {
        string newPath = actionToRebind.bindings[bindingIndex].effectivePath;
        rebindingOperation.Dispose();
        SyncSecondaryActions(newPath);
        ResolveDuplicates(newPath);
        InputManager.Instance.InputActions.Enable();
        
        rebindButton.interactable = true;
        OnAnyBindingChanged?.Invoke();
    }

    void SyncSecondaryActions(string newPath)
    {
        if (secondaryActions == null || InputManager.Instance?.InputActions?.asset == null) return;

        foreach (string path in secondaryActions)
        {
            if (string.IsNullOrEmpty(path)) continue;

            var secondaryAction = InputManager.Instance.InputActions.asset.FindAction(path);
            if (secondaryAction != null)
            {
                for (int i = 0; i < secondaryAction.bindings.Count; i++)
                {
                    string bindingGroups = secondaryAction.bindings[i].groups ?? "";
                    if (bindingGroups.Contains(controlScheme) || string.IsNullOrEmpty(bindingGroups)) secondaryAction.ApplyBindingOverride(i, newPath);
                }
            }
        }
    }
    void RebindCancelled()
    {
        rebindingOperation.Dispose();
        InputManager.Instance.InputActions.Enable();
        rebindButton.interactable = true;
        UpdateUI();
    }

    void ResolveDuplicates(string newBindingPath)
    {
        if (string.IsNullOrEmpty(newBindingPath) || InputManager.Instance?.InputActions?.asset == null) return; 

        foreach (InputAction action in InputManager.Instance.InputActions.asset)
        {
            if (action == actionToRebind) continue;
            bool isSecondary = false;
            if (secondaryActions != null)
            {
                foreach (var sPath in secondaryActions)
                {
                    if (string.IsNullOrEmpty(sPath)) continue;
                    if (action.name == sPath.Split('/')[^1])
                    {
                        isSecondary = true;
                        break;
                    }
                }
            }

            if (isSecondary) continue;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                InputBinding binding = action.bindings[i];
                if (!string.IsNullOrEmpty(binding.effectivePath) && binding.effectivePath == newBindingPath) action.ApplyBindingOverride(i, "");
            }
        }
    }

    void UpdateUI()
    {
        if (actionToRebind == null) return;
        string displayString = InputControlPath.ToHumanReadableString(actionToRebind.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        bindButtonText.text = string.IsNullOrEmpty(displayString) ? " " : displayString;
    }

    void OnDestroy() => rebindButton.onClick.RemoveListener(StartRebinding);

    public static void RefreshAllUI() => OnAnyBindingChanged?.Invoke();
}
