using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{

    [SerializeField] private Button menuButton;
    [SerializeField] private Button readyButton;

    private void Start()
    {

        menuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MenuScene);
        });

        readyButton.onClick.AddListener(() =>
        {
            CharacterSelectionReady.Instance.UpdatePlayersReady();
        });
    }

}
