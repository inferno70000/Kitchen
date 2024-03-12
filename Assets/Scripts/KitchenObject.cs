using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO KitchenObjectSO;
    [SerializeField] private IKitchenObjectParent kitchenObjectParent;

    /// <summary>
    /// Get KitchenObjectSO form this KitchenObject
    /// </summary>
    /// <returns></returns>
    public KitchenObjectSO GetKitchenScriptableSO()
    {
        return KitchenObjectSO;
    }

    /// <summary>
    /// Get clear counter which is this KitchenObject's parent
    /// </summary>
    /// <returns></returns>
    public IKitchenObjectParent GetClearCounter()
    {
        return kitchenObjectParent;
    }

    /// <summary>
    /// Set this KitchenObject to new parent
    /// </summary>
    /// <param name="kitchenObjectParent">New parent</param>
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        //clear KitchenObject on old parent if existed
        this.kitchenObjectParent?.ClearKitchenObject();

        //set new parent
        this.kitchenObjectParent = kitchenObjectParent;

        //This KitchenObject already has a parent
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("KitchenObjectParent already has a KitchenObject.");
        }

        kitchenObjectParent.SetKitchenObject(this);
        transform.parent = kitchenObjectParent.GetFollowingObjectTopPoint();
        transform.localPosition = Vector3.zero;
    }
}
