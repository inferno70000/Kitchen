using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectParent
{
    /// <summary>
    /// Clear kitchen object on the parent
    /// </summary>
    public void ClearKitchenObject();
    /// <summary>
    /// Get kitchen object from the parent
    /// </summary>
    /// <returns>KitchenObject</returns>
    public KitchenObject GetKitchenObject();
    /// <summary>
    /// Set kitchen object on the parent
    /// </summary>
    /// <param name="kitchenObject">Kitchen object want to set</param>
    public void SetKitchenObject(KitchenObject kitchenObject);
    /// <summary>
    /// Get the holding object point
    /// </summary>
    /// <returns>Transform of the point</returns>
    public Transform GetFollowingObjectTopPoint();
    /// <summary>
    /// Get status is parent has object or not
    /// </summary>
    /// <returns>Boolean</returns>
    public bool HasKitchenObject();
}
