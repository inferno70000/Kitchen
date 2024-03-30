using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnSpawn;

    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnPickupSomething;
    public event EventHandler<OnSelectedCounterChangedEventArg> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArg
    {
        public BaseCounter selectedCouter;
    }

    private const string COUNTER_LAYERMASK = "Interactable";
    private const string KITCHEN_OBJECT_HOLD_POINT = "KitchenObjectHoldPoint";

    [SerializeField] private float speed = 5f;
    private bool isWalking;
    private LayerMask counterLayerMask;
    private BaseCounter selectedCounter;

    private KitchenObject kitchenObject;
    private Transform kitchenObjectHoldPoint;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        LocalInstance = this;
        OnSpawn?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        counterLayerMask = LayerMask.GetMask(COUNTER_LAYERMASK);
        kitchenObjectHoldPoint = transform.Find(KITCHEN_OBJECT_HOLD_POINT);
        InputManager.Instance.OnInteractAction += InputManager_OnInteractAction;
        InputManager.Instance.OnInteractAlternativeAction += InputManager_OnInteractAlternativeAction;
    }

    private void InputManager_OnInteractAlternativeAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternative(this);
        }
    }

    private void InputManager_OnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        Move();
        Interact();
    }

    private void Interact()
    {
        float distance = 1.5f;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, distance, counterLayerMask))
        {
            if (hitInfo.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                SetSelectedCounter(baseCounter);
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
        Vector2 inputVector = InputManager.Instance.GetMovementVectorNormalized();

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

    /// <summary>
    /// Get status player is walking or not
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArg
        {
            selectedCouter = selectedCounter
        });
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        OnPickupSomething?.Invoke(this, EventArgs.Empty);
    }

    public Transform GetFollowingObjectTopPoint()
    {
        return kitchenObjectHoldPoint;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public static void ResetStaticData()
    {
        OnSpawn = null;
        OnPickupSomething = null;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
