using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelLoader : ScriptableObject
{
    [MenuItem("GDFR/Loading Screen")]
    static void LoadLoadingScrene()
    {
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/Logo.unity");
    }

    [MenuItem("GDFR/Main Menu")]
    static void LoadMenu()
    {
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/MainMenu.unity");
    }

    [MenuItem("GDFR/Setup")]
    static void LoadSetup()
    {
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/NewGame.unity");
    }

    [MenuItem("GDFR/Game")]
    static void LoadGame()
    {
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/MainGame.unity");
    }

    [MenuItem("GDFR/Help Menu")]
    static void LoadHelpMenu()
    {
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/Help_Additive.unity");
    }
}