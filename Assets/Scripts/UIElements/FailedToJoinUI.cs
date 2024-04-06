using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FailedToJoinUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI failedMessage;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        GameNetworkManager.Instance.OnFailedToJoin += GameNetworkManager_OnFailedToJoin;

        Hide();
    }

    private void GameNetworkManager_OnFailedToJoin(object sender, System.EventArgs e)
    {
        failedMessage.text = NetworkManager.Singleton.DisconnectReason;

        if (failedMessage.text == "")
        {
            failedMessage.text = "Failed to connect.";
        }

        Show();
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
        GameNetworkManager.Instance.OnFailedToJoin -= GameNetworkManager_OnFailedToJoin;
    }
}
