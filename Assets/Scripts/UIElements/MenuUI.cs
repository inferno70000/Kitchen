using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        multiplayerButton.onClick.AddListener(() =>
        {
            GameNetworkManager.IsMultiplayer = true;

            Loader.Load(Loader.Scene.LobbyScene);
        });

        singleplayerButton.onClick.AddListener(() =>
        {
            GameNetworkManager.IsMultiplayer = false;

            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        });

        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        singleplayerButton.Select();
    }
}
