using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArg> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArg
    {
        public ClearCounter selectedCouter;
    }

    private const string COUNTER_LAYERMASK = "Interactable";

    [SerializeField] private InputManager inputManager;
    [SerializeField] private float speed = 5f;
    private bool isWalking;
    private LayerMask counterLayerMask;
    private ClearCounter selectedCounter;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance.");
        }

        Instance = this;
    }

    private void Start()
    {
        counterLayerMask = LayerMask.GetMask(COUNTER_LAYERMASK);
        inputManager.OnInteractAction += InputManager_OnInteractAction;
    }

    private void InputManager_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    private void Update()
    {
        Move();
        Interact();
    }

    private void Interact()
    {
        float distance = 1.5f;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, distance, counterLayerMask))
        {
            if (hitInfo.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                SetSelectedCounter(clearCounter);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void Move()
    {
        Vector2 inputVector = inputManager.GetMovementVectorNormalized();

        //update bool for PlayerAnimation's move animation
        isWalking = inputVector != Vector2.zero;

        //transform to 3d world directrion
        Vector3 dir = new(inputVector.x, 0, inputVector.y);

        //move
        transform.position += speed * Time.deltaTime * dir;

        //rotate player to forward movement
        float rotationSpeed = 10f;
        if (dir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotationSpeed);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArg
        {
            selectedCouter = selectedCounter
        });
    }
}
