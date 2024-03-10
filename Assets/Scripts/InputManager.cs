using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event EventHandler OnInteractAction;

    private PlayerInput playerInput;
    private PlayerInput.PlayerActions playerActions;

    private void Awake()
    {
        playerInput = new();
        playerActions = playerInput.Player;
        playerActions.Enable();
        playerActions.Interact.performed += Interact_performed;
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
