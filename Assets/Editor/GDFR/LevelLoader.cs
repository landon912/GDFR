using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class LevelLoader : ScriptableObject
{
    [MenuItem("GDFR/Loading Screen")]
    static void LoadLoadingScrene()
    {
        SaveDirtyScenesIfWanted();
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/Logo.unity");
    }

    [MenuItem("GDFR/Main Menu")]
    static void LoadMenu()
    {
        SaveDirtyScenesIfWanted();
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/MainMenu.unity");
    }

    [MenuItem("GDFR/Setup")]
    static void LoadSetup()
    {
        SaveDirtyScenesIfWanted();
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/NewGame.unity");
    }

    [MenuItem("GDFR/Game")]
    static void LoadGame()
    {
        SaveDirtyScenesIfWanted();
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/MainGame.unity");
    }

    [MenuItem("GDFR/Help Menu")]
    static void LoadHelpMenu()
    {
        SaveDirtyScenesIfWanted();
        EditorSceneManager.OpenScene("Assets/GDFR/Scenes/Help_Additive.unity");
    }

    static void SaveDirtyScenesIfWanted()
    {
        List<Scene> dirtyScenes = new List<Scene>();
        int numScenes = EditorSceneManager.sceneCount;
        for (int i = 0; i < numScenes; i++)
        {
            if (EditorSceneManager.GetSceneAt(i).isDirty)
            {
                dirtyScenes.Add(EditorSceneManager.GetSceneAt(i));
            }
        }
        EditorSceneManager.SaveModifiedScenesIfUserWantsTo(dirtyScenes.ToArray());
    }
}