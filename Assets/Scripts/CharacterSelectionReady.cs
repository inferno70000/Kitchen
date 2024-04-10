using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionReady : NetworkBehaviour
{

    public static CharacterSelectionReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;

    private Dictionary<ulong, bool> playersReadyDictionary = new();


    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePlayersReady()
    {
        UpdatePlayersReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayersReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allPlayersReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playersReadyDictionary.ContainsKey(clientId) || !playersReadyDictionary[clientId])
            {
                allPlayersReady = false;
            }
        }

        if (allPlayersReady)
        {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playersReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playersReadyDictionary.ContainsKey(clientId) && playersReadyDictionary[clientId];
    }
}
