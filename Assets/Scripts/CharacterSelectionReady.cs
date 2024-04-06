using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionReady : NetworkBehaviour
{
    [SerializeField] private Button readyButton;
    private Dictionary<ulong, bool> playersReadyDictionary = new();

    private void Start()
    {
        readyButton.onClick.AddListener(() =>
        {
            UpdatePlayersReadyServerRpc();
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayersReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
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
}
