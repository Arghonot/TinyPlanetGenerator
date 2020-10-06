using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchVerifier : EditorWindow
{
    [MenuItem("Verifier/Start")]
    public static void Create()
    {
        GetWindow<SceneSwitchVerifier>();
    }
    void Start()
    {
        EditorSceneManager.sceneDirtied += EditorSceneManager_sceneDirtied;
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;
        EditorSceneManager.sceneSaving += EditorSceneManager_sceneSaving;
        EditorSceneManager.sceneClosed += EditorSceneManager_sceneClosed;
        EditorSceneManager.sceneClosing += OnStartLoadingScene;
        EditorSceneManager.sceneOpened += OnLoadedScene;
        Debug.Log("skjsdksj");
    }

    private void EditorSceneManager_sceneDirtied(Scene scene)
    {
        Debug.Log("EditorSceneManager_sceneDirtied");
    }

    private void EditorSceneManager_sceneSaved(Scene scene)
    {
        Debug.Log("EditorSceneManager_sceneSaved");
    }

    private void EditorSceneManager_sceneSaving(Scene scene, string path)
    {
        Debug.Log("EditorSceneManager_sceneSaving");
    }

    private void EditorSceneManager_sceneClosed(Scene scene)
    {
        Debug.Log("EditorSceneManager_sceneClosed");
    }

    void OnLoadedScene(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
    {
        Debug.Log("OnLoadedScene");
    }

    void OnStartLoadingScene(Scene scene, bool removingscene)
    {
        Debug.Log("OnStartLoadingScene");
    }
}