using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForOtherPlayersUI : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.OnLocalPlayerReady += GameManager_OnIsLocalPlayerReadyChanged;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownState())
        {
            Hide();
        }
    }

    private void GameManager_OnIsLocalPlayerReadyChanged(object sender, System.EventArgs e)
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
}
