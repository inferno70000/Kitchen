using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO KitchenObjectSO;
    [SerializeField] private IKitchenObjectParent kitchenObjectParent;
    private FollowTranform followTranform;

    protected virtual void Awake()
    {
        followTranform = GetComponent<FollowTranform>();
    }

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

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    /// <summary>
    /// Set this KitchenObject to new parent
    /// </summary>
    /// <param name="kitchenObjectParent">New parent</param>
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out var kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

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

        followTranform.SetTargetTransform(kitchenObjectParent.GetFollowingObjectTopPoint());
    }

    /// <summary>
    /// Destroy this kitchen object
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Clear kitchen object on this' parent
    /// </summary>
    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        GameNetworkManager.Instance.DestroyKitchenObject(kitchenObject);
    }

    /// <summary>
    /// Spawn a new kitchen object
    /// </summary>
    /// <param name="kitchenObjectSO">Kitchen object needs to spawn</param>
    /// <param name="kitchenObjectParent">Parent of the new kitchen object</param>
    /// <returns>KitchenObject</returns>
    public static void Spawn(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        GameNetworkManager.Instance.Spawn(kitchenObjectSO, kitchenObjectParent);
    }

    /// <summary>
    /// Try getting plateKitchenObject. if existed, return out plateKitchenObject
    /// </summary>
    /// <param name="plateKitchenObject">asign plateKitchenObject to this param</param>
    /// <returns>bool</returns>
    public bool TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }

        plateKitchenObject = null;
        return false;
    }
}
