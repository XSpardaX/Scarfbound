#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuManager))]
public class MenuManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply Bloodlines Buttons"))
            MainMenuBloodlinesButtonUtility.ApplyToScene();

        EditorGUILayout.HelpBox(
            "Open the Main Menu scene, select Menu Manager, click the button above, then save the scene. " +
            "Legacy buttons are replaced in-place with Bloodlines prefab instances at the same positions.",
            MessageType.Info);
    }
}
#endif
