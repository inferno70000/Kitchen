using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.PlayerActions playerActions;

    private void Awake()
    {
        playerInput = new();
        playerActions = playerInput.Player;
        playerActions.Enable();
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
