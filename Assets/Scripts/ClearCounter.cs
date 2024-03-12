using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    public KitchenObjectSO kitchenObjectSO;

    private const string KITCHEN_OBJECT_TOP_POINT = "KitchenObjectTopPoint";

    private KitchenObject kitchenObject;
    private Transform kitchenObjectTopPoint;

    private void Start()
    {
        kitchenObjectTopPoint = transform.Find(KITCHEN_OBJECT_TOP_POINT);
    }

    public void Interact(Player player)
    {
        if (kitchenObject == null)
        {
            Transform kitchenTransform = Instantiate(kitchenObjectSO.prefab, kitchenObjectTopPoint);
            kitchenTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
        else
        {
            kitchenObject.SetKitchenObjectParent(player);
        }
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
