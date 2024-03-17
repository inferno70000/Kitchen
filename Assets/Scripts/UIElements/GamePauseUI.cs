using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePause();
        });
        menuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGamePause += GameManager_OnGamePause;
        GameManager.Instance.OnGameUnPause += GameManager_OnGameUnPause;

        Hide();
    }

    private void GameManager_OnGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGamePause(object sender, System.EventArgs e)
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
