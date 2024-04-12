using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int clientIndex;
    [SerializeField] private GameObject readyText;
    [SerializeField] private Button kickButton;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private TextMeshProUGUI playerNameText;

    private void Start()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = GameNetworkManager.Instance.GetPlayerDataFromIndex(clientIndex);

            LobbyManager.Instance.KickPlayer(playerData.playerId.ToString());

            if (playerData.clientId != 0)
            {
                GameNetworkManager.Instance.DisconnectClient(playerData.clientId);
            }
            else
            {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MenuScene);
            }
        });

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        GameNetworkManager.Instance.playerDataList.OnListChanged += PlayerDataList_OnListChanged;
        CharacterSelectionReady.Instance.OnReadyChanged += CharacterSelectionReady_OnReadyChanged;

        Hide();

        Invoke(nameof(UpdatePlayer), 0.2f);
    }

    private void PlayerDataList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        UpdatePlayer();
    }

    private void CharacterSelectionReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (GameNetworkManager.Instance.IsPlayerConnected(clientIndex))
        {
            Show();

            PlayerData playerData = GameNetworkManager.Instance.GetPlayerDataFromIndex(clientIndex);

            readyText.SetActive(CharacterSelectionReady.Instance.IsPlayerReady(playerData.clientId));

            playerVisual.SetPlayerColor(GameNetworkManager.Instance.GetColorFromColorId(playerData.colorId));

            playerNameText.text = playerData.name.ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameNetworkManager.Instance.playerDataList.OnListChanged -= PlayerDataList_OnListChanged;
    }
}
