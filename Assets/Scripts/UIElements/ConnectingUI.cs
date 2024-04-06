using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        GameNetworkManager.Instance.OnTryingToJoin += GameNetworkManager_OnTryingToJoin;
        GameNetworkManager.Instance.OnFailedToJoin += GameNetworkManager_OnFailedToJoin;

        Hide();
    }

    private void GameNetworkManager_OnFailedToJoin(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameNetworkManager_OnTryingToJoin(object sender, System.EventArgs e)
    {
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
        GameNetworkManager.Instance.OnTryingToJoin -= GameNetworkManager_OnTryingToJoin;
        GameNetworkManager.Instance.OnFailedToJoin -= GameNetworkManager_OnFailedToJoin;
    }
}
