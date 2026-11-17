using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Editor-only: pressing Play always starts the game's first build scene (Assets/Scenes/SampleScene), even when the
// editor reopened an empty "Untitled" scene. The whole game is built in code from that scene, so Play from any other
// scene shows a blank screen. Does not affect player builds.
[InitializeOnLoad]
public static class PlayModeStartScene
{
    static PlayModeStartScene()
    {
        EditorApplication.delayCall += Apply;
    }

    private static void Apply()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        if (scenes.Length == 0 || string.IsNullOrEmpty(scenes[0].path))
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        SceneAsset gameScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[0].path);
        if (gameScene == null)
        {
            Debug.LogWarning($"Play-mode start scene not found at {scenes[0].path}; Play will use the open scene.");
        }

        EditorSceneManager.playModeStartScene = gameScene;
    }
}
