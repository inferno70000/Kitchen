using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button meneuButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;

    private void Start()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });

        quickJoinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.QuickJoin();
        });

        meneuButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MenuScene);
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinLobbyWithCode(joinCodeInputField.text.ToUpper());    
        });

        playerNameInputField.text = GameNetworkManager.Instance.GetPlayerName();

        playerNameInputField.onValueChanged.AddListener((string playerName) =>
        {
            GameNetworkManager.Instance.SetPlayerName(playerName);
        });
    }
}
