using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float speed = 5f;
    private bool isWalking;

    private void Update()
    {
        Vector2 inputVector = inputManager.GetMovementVectorNormalized();

        isWalking = inputVector != Vector2.zero;

        Vector3 dir = new(inputVector.x, 0, inputVector.y);

        transform.position +=  speed * Time.deltaTime * dir;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotationSpeed);

    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
