using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternativeAction;

    private PlayerInput playerInput;
    private PlayerInput.PlayerActions playerActions;

    private void Awake()
    {
        playerInput = new();
        playerActions = playerInput.Player;
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
}
