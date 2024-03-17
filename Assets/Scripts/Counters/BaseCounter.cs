using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnGetKitchenObject;

    private const string KITCHEN_OBJECT_TOP_POINT = "KitchenObjectTopPoint";

    private KitchenObject kitchenObject;
    private Transform kitchenObjectTopPoint;

    public static void ResetStaticData()
    {
        OnGetKitchenObject = null;
    }

    private void Start()
    {
        kitchenObjectTopPoint = transform.Find(KITCHEN_OBJECT_TOP_POINT);
    }

    /// <summary>
    /// Define interaction of the counter
    /// </summary>
    /// <param name="player">Instance of player</param>
    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact()");
    }
    
    /// <summary>
    /// Define interaction aternate of the counter
    /// </summary>
    /// <param name="player">Instance of player</param>
    public virtual void InteractAlternative(Player player)
    {
        //Debug.LogError("BaseCounter.InteractAlternative()");
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

        OnGetKitchenObject?.Invoke(this, EventArgs.Empty);  
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
