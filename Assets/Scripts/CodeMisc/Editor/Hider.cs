using UnityEngine;
using UnityEditor;


/// <summary>
/// This class is a tool that helps delete objects that are hidden
/// </summary>
public class Hider : EditorWindow
{
    public string ObjectName = "Sphere";
    [MenuItem("SceneCleaner/Start")]
    public static void Create()
    {
        GetWindow<Hider>();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("This is a tool to help you delete the hidden gameobjects.");

        ObjectName = EditorGUILayout.TextField(
            ObjectName);

        if (GUILayout.Button("Select hidden GO"))
        {
            Selection.activeGameObject = GameObject.Find(ObjectName);
        }

        if (GUILayout.Button("Destory Selected Object"))
        {
            DestroyImmediate(Selection.activeObject);
        }
    }
}