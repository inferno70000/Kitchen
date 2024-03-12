using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public KitchenObjectSO kitchenObjectSO;

    private const string KITCHEN_OBJECT_TOP_POINT = "KitchenObjectTopPoint";

    private KitchenObject kitchenObject;
    private Transform kitchenObjectTopPoint;
    private void Start()
    {
        kitchenObjectTopPoint = transform.Find(KITCHEN_OBJECT_TOP_POINT);
    }

    public virtual void Interact(Player player)
    {

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
    }

    public Transform GetFollowingObjectTopPoint()
    {
        return kitchenObjectTopPoint;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
