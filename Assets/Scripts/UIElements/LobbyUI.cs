using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Start()
    {
        createGameButton.onClick.AddListener(() =>
        {
            GameNetworkManager.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectionScene);
        });

        joinGameButton.onClick.AddListener(() =>
        {
            GameNetworkManager.Instance.StartClient();
        });
    }
}
