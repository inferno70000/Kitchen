using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI message;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        GameNetworkManager.Instance.OnFailedToJoin += GameNetworkManager_OnFailedToJoin;
        LobbyManager.Instance.OnCreateLobby += LobbyManager_OnCreateLobby;
        LobbyManager.Instance.OnJoinLobby += LobbyManager_OnJoinLobby;
        LobbyManager.Instance.OnJoinFailed += LobbyManager_OnJoinFailed;
        LobbyManager.Instance.OnCreateFailed += LobbyManager_OnCreateFailed;
        LobbyManager.Instance.OnQuickJoinFailed += LobbyManager_OnQuickJoinFailed;

        Hide();
    }

    private void LobbyManager_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        Show("Coud not find a lobby to Quick Join!");
    }

    private void LobbyManager_OnCreateFailed(object sender, System.EventArgs e)
    {
        Show("Failed to create a Lobby!");
    }

    private void LobbyManager_OnJoinFailed(object sender, System.EventArgs e)
    {
        Show("Failed to join a Lobby!");
    }

    private void LobbyManager_OnJoinLobby(object sender, System.EventArgs e)
    {
        Show("Joining a Lobby...");
    }

    private void LobbyManager_OnCreateLobby(object sender, System.EventArgs e)
    {
        Show("Connecting a Lobby...");
    }

    private void GameNetworkManager_OnFailedToJoin(object sender, System.EventArgs e)
    {
        Show(NetworkManager.Singleton.DisconnectReason);
    }

    private void Show(string text)
    {
        message.text = text;

        if (message.text == "")
        {
            message.text = "Failed to connect!";
        }

        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameNetworkManager.Instance.OnFailedToJoin -= GameNetworkManager_OnFailedToJoin;
    }
}
