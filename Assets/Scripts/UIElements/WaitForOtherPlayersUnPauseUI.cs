using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForOtherPlayersUnPauseUI : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.OnMultiPlayerGamePause += GameManager_OnMultiPlayerGamePause;
        GameManager.Instance.OnMultiPlayerGameUnPause += GameManager_OnMultiPlayerGameUnPause;

        Hide();
    }

    private void GameManager_OnMultiPlayerGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnMultiPlayerGamePause(object sender, System.EventArgs e)
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
