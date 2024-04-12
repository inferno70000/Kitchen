using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    private Lobby lobby;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinLobbyWithId(lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyName.text = lobby.Name;
    }
}
