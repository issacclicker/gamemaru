using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public enum Scene{
        MainMenuScene,
        netcode_test2,
        LoadingScene,
        LobbyScene,
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene){
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }

    public static void LoadNetwork(Scene targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(),LoadSceneMode.Single);
    }

    public static void LoaderCallback(){
        SceneManager.LoadScene(targetScene.ToString());
    }
}
