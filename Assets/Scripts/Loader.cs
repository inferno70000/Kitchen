using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MenuScene,
        LoadingScene,
        GameScene,
        CharacterSelectionScene,
        LobbyScene
    }

    private static Scene scene;

    /// <summary>
    /// Load scene
    /// </summary>
    /// <param name="scene">scene want to load</param>
    public static void Load(Scene scene)
    {
        Loader.scene = scene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    /// <summary>
    /// Load scene on network
    /// </summary>
    /// <param name="scene">scene want to load</param>
    public static void LoadNetwork(Scene scene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }

    /// <summary>
    /// Pass scene to LoadingScene's loadCallBack
    /// </summary>
    public static void LoadCallBack()
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
