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

    private int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSO)
    {
        return listKitchenObjectSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
    {
        return listKitchenObjectSO.kitchenObjectSOList[index];
    }
}
