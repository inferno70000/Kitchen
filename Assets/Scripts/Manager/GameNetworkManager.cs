using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkBehaviour
{
    public static GameNetworkManager Instance;

    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;

    private const int MAX_PLAYERS = 4;

    [SerializeField] private ListKitchenObjectSO listKitchenObjectSO;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoin?.Invoke(this, EventArgs.Empty);
    }

    public void StartClient()
    {
        OnTryingToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectionScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already played.";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full.";
            return;
        }

        connectionApprovalResponse.Approved = true;
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

    /// <summary>
    /// Get index of kitchen object SO
    /// </summary>
    /// <param name="kitchenObjectSO">kitchenObject want to get index</param>
    /// <returns></returns>
    public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSO)
    {
        return listKitchenObjectSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    /// <summary>
    /// Get kitchen object SO by index
    /// </summary>
    /// <param name="index">index of kitchen object SO</param>
    /// <returns></returns>
    public KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
    {
        return listKitchenObjectSO.kitchenObjectSOList[index];
    }

    /// <summary>
    /// Destroy Kitchen Object
    /// </summary>
    /// <param name="kitchenObject">kitchen object want to destroy</param>
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
