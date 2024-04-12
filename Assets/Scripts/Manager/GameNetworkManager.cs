using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkBehaviour
{
    public static GameNetworkManager Instance;

    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;

    public NetworkList<PlayerData> playerDataList { get; private set; }

    public const int MAX_PLAYERS = 4;

    private const string PLAYER_PREFS_PLAYER_NAME = "PlayerName";

    [SerializeField] private ListKitchenObjectSO listKitchenObjectSO;
    [SerializeField] private List<Color> colorList = new();
    private string playerName;

    private void Awake()
    {

        Instance = this;

        DontDestroyOnLoad(this);

        playerDataList = new();

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME, "Player " + UnityEngine.Random.Range(0, 1000));
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    public string GetPlayerName()
    {
        return playerName;  
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME , playerName);
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].clientId == clientId)
            {
                playerDataList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
    {
        playerDataList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColor(),
        });
        ChangePlayerNameServerRpc(GetPlayerName());
        ChangePlayerId(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoin?.Invoke(this, EventArgs.Empty);
    }

    public void StartClient()
    {
        OnTryingToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong obj)
    {
        ChangePlayerName(GetPlayerName());
        ChangePlayerId(AuthenticationService.Instance.PlayerId);
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

    public bool IsPlayerConnected(int index)
    {
        return index < playerDataList.Count;
    }

    public PlayerData GetPlayerDataFromIndex(int index)
    {
        return playerDataList[index];
    }

    public Color GetColorFromColorId(int colorId)
    {
        return colorList[colorId];
    }

    public int GetPlayerIndexDataFromLocalId(ulong clienId)
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].clientId == clienId)
            {
                return i;
            }
        }

        return -1;
    }

    public PlayerData GetPlayerDataFromLocalId(ulong clienId)
    {
        foreach (PlayerData playerData in playerDataList)
        {
            if (playerData.clientId == clienId)
            {
                return playerData;
            }
        }

        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromLocalId(NetworkManager.Singleton.LocalClientId);
    }

    public void ChangePlayerId(string playerId)
    {
        ChangePlayerIdServerRpc(playerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerIndexDataFromLocalId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataList[playerDataIndex] = playerData;
    }

    public void ChangePlayerName(string playerName)
    {
        ChangePlayerNameServerRpc(playerName);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerIndexDataFromLocalId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataList[playerDataIndex];

        playerData.name = playerName;

        playerDataList[playerDataIndex] = playerData;
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);

    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            //Color is already selected
            return;
        }

        int playerDataIndex = GetPlayerIndexDataFromLocalId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataList[playerDataIndex];

        playerData.colorId = colorId;

        playerDataList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataList)
        {
            if (colorId == playerData.colorId)
            {
                return false;
            }
        }

        return true;
    }

    private int GetFirstUnusedColor()
    {
        for(int i = 0; i < colorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }

    public void DisconnectClient(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);

        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
