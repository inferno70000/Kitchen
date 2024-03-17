using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MenuScene,
        LoadingScene,
        GameScene,
    }

    private static Scene scene;

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
