using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternativeAction;
    public event EventHandler OnReBinding;

    private const string PLAYER_PREFS_BINDING = "Binding";

    private PlayerInput playerInput;
    private PlayerInput.PlayerActions playerActions;

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
        GamepadInteract,
        GamepadInteractAlternate,
        GamepadPause,
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one InputManager instance.");
        }

        Instance = this;

        playerInput = new();
        playerActions = playerInput.Player;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDING))
        {
            playerInput.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDING));
        }

        playerActions.Enable();
        playerActions.Interact.performed += Interact_performed;
        playerActions.InteractAlternative.performed += InteractAlternative_performed;
        playerActions.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerActions.Interact.performed -= Interact_performed;
        playerActions.Pause.performed -= Pause_performed;
        playerActions.InteractAlternative.performed -= InteractAlternative_performed;

        playerInput.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        GameManager.Instance.TogglePause();
    }

    private void InteractAlternative_performed(InputAction.CallbackContext obj)
    {
        OnInteractAlternativeAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Get movement vector normalized from input system
    /// </summary>
    /// <returns>Vector 2</returns>
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerActions.Movement.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    /// <summary>
    /// Get bindText with given binding
    /// </summary>
    /// <param name="binding">Binding want to get text</param>
    /// <returns></returns>
    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.MoveUp:
                return playerActions.Movement.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return playerActions.Movement.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return playerActions.Movement.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return playerActions.Movement.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerActions.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerActions.InteractAlternative.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerActions.Pause.bindings[0].ToDisplayString();
            case Binding.GamepadInteract:
                return playerActions.Interact.bindings[1].ToDisplayString();
            case Binding.GamepadInteractAlternate:
                return playerActions.InteractAlternative.bindings[1].ToDisplayString();
            case Binding.GamepadPause:
                return playerActions.Pause.bindings[1].ToDisplayString();
            default:
                return null;
        }
    }

    /// <summary>
    /// Rebinding
    /// </summary>
    /// <param name="binding">Binding want to change</param>
    /// <param name="action">action want to execute when rebinding completed</param>
    public void Rebinding(Binding binding, Action action)
    {
        InputAction inputAction = new();
        int bindingIndex = 0;

        switch (binding)
        {
            case Binding.MoveUp:
                inputAction = playerActions.Movement;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = playerActions.Movement;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = playerActions.Movement;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = playerActions.Movement;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerActions.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerActions.InteractAlternative;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerActions.Pause;
                bindingIndex = 0;
                break;
            case Binding.GamepadInteract:
                inputAction = playerActions.Interact;
                bindingIndex = 1;
                break;
            case Binding.GamepadInteractAlternate:
                inputAction = playerActions.InteractAlternative;
                bindingIndex = 1;
                break;
            case Binding.GamepadPause:
                inputAction = playerActions.Pause;
                bindingIndex = 1;
                break;
            default:
                break;
        }

        playerActions.Disable();

        inputAction.PerformInteractiveRebinding(bindingIndex).
            WithControlsExcluding("Mouse").
            OnComplete(callback =>
        {
            action();
            callback.Dispose();
            PlayerPrefs.SetString(PLAYER_PREFS_BINDING, playerInput.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();

            OnReBinding?.Invoke(this, EventArgs.Empty); 
        }).Start();

        playerActions.Enable();
    }
}
