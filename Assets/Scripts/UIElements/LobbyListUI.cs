using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private GameObject lobbySingleUI;
    [SerializeField] private Transform lobbySingleUIContainer;

    private void Start()
    {
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        foreach(Transform lobbySingleUI in lobbySingleUIContainer)
        {
            if (lobbySingleUI.gameObject != this.lobbySingleUI)
            {
                Destroy(lobbySingleUI.gameObject);
            }
        }

        foreach(Lobby lobby in e.LobbyList)
        {
            GameObject newLobbySingleUI = Instantiate(lobbySingleUI, lobbySingleUIContainer);
            newLobbySingleUI.GetComponent<LobbySingleUI>().SetLobby(lobby);
            newLobbySingleUI.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }
}
