using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObjectNetworkManager : NetworkBehaviour
{
    public static KitchenObjectNetworkManager Instance;

    [SerializeField] private ListKitchenObjectSO listKitchenObjectSO;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Spawn kitchen object by server rpc
    /// </summary>
    /// <param name="kitchenObjectSO">kitchen object to spawn</param>
    /// <param name="kitchenObjectParent">parent of spawned kitchen object</param>
    public void Spawn(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnServerRpc(GetKitchenObjectIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc(int indexKitchenObject, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(indexKitchenObject);
        Transform kitchenTransform = Instantiate(kitchenObjectSO.prefab);
        kitchenTransform.GetComponent<NetworkObject>().Spawn();

        KitchenObject kitchenObject = kitchenTransform.GetComponent<KitchenObject>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSO)
    {
        return listKitchenObjectSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
    {
        return listKitchenObjectSO.kitchenObjectSOList[index];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)] 
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRpc(networkObjectReference);

        kitchenObject.DestroySelf();
    }

    [ClientRpc] 
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }
}
