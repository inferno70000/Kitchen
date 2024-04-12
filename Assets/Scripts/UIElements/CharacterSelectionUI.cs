using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{

    [SerializeField] private Button menuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCode;

    private void Start()
    {
        menuButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MenuScene);
        });

        readyButton.onClick.AddListener(() =>
        {
            CharacterSelectionReady.Instance.UpdatePlayersReady();
        });

        Lobby joinedLobby = LobbyManager.Instance.GetJoinedLobby();

        lobbyName.text = "Lobby name: " + joinedLobby.Name;
        lobbyCode.text = "Lobby code: " + joinedLobby.LobbyCode;
    }
}
