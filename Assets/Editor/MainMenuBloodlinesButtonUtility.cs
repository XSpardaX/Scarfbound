#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MainMenuBloodlinesButtonUtility
{
    private const string MainMenuSceneName = "Main Menu";
    private const string ButtonPrefabPath = "Assets/Alebardium/Bloodlines UI/Prefabs/Button/Button 1 (Red).prefab";
    private const float MinButtonWidth = 220f;
    private const float ButtonHeight = 59.333332f;

    private readonly struct ButtonSpec
    {
        public readonly string ParentName;
        public readonly string ChildName;
        public readonly string Label;

        public ButtonSpec(string parentName, string childName, string label)
        {
            ParentName = parentName;
            ChildName = childName;
            Label = label;
        }
    }

    private static readonly ButtonSpec[] MainAndLevelButtons =
    {
        new ButtonSpec("Main Buttons", "Play", "Start Game"),
        new ButtonSpec("Main Buttons", "Instructions", "How to play"),
        new ButtonSpec("Main Buttons", "Quit", "Quit Game"),
        new ButtonSpec("Level Select", "Level 1", "Level 1"),
        new ButtonSpec("Level Select", "Level 2", "Level 2"),
        new ButtonSpec("Level Select", "Level 3", "Level 3"),
        new ButtonSpec("Level Select", "Back", "Back"),
    };

    [MenuItem("Scarfbound/Main Menu/Apply Bloodlines Buttons")]
    public static void ApplyFromMenu()
    {
        ApplyToScene();
    }

    public static bool ApplyToScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != MainMenuSceneName)
        {
            EditorUtility.DisplayDialog(
                "Wrong Scene",
                $"Open the \"{MainMenuSceneName}\" scene before applying Bloodlines buttons.",
                "OK");
            return false;
        }

        GameObject buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ButtonPrefabPath);
        if (buttonPrefab == null)
        {
            EditorUtility.DisplayDialog(
                "Missing Prefab",
                $"Could not find Bloodlines button prefab at:\n{ButtonPrefabPath}",
                "OK");
            return false;
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Missing Canvas", "No Canvas found in the active scene.", "OK");
            return false;
        }

        Undo.SetCurrentGroupName("Apply Bloodlines Main Menu Buttons");
        int undoGroup = Undo.GetCurrentGroup();

        Transform canvasTransform = canvas.transform;
        foreach (ButtonSpec spec in MainAndLevelButtons)
        {
            Transform parent = canvasTransform.Find(spec.ParentName);
            if (parent == null)
            {
                Debug.LogWarning($"MainMenuBloodlinesButtonUtility: Missing \"{spec.ParentName}\" under Canvas.");
                continue;
            }

            ReplaceButton(parent, spec.ChildName, spec.Label, buttonPrefab);
        }

        foreach (Transform child in canvasTransform)
        {
            if (child.name != "Panel" || child.Find("Button") == null)
                continue;

            ReplaceButton(child, "Button", "Back to menu", buttonPrefab);
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Bloodlines main menu buttons applied. Save the scene to keep the changes.");
        return true;
    }

    private static void ReplaceButton(Transform parent, string childName, string label, GameObject buttonPrefab)
    {
        Transform oldButton = parent.Find(childName);
        if (oldButton == null)
        {
            Debug.LogWarning($"MainMenuBloodlinesButtonUtility: Missing \"{childName}\" under {parent.name}.");
            return;
        }

        if (IsBloodlinesButton(oldButton.gameObject))
        {
            SetLabel(oldButton, label);
            return;
        }

        RectTransform oldRect = oldButton as RectTransform;
        if (oldRect == null)
            oldRect = oldButton.GetComponent<RectTransform>();

        int siblingIndex = oldButton.GetSiblingIndex();
        Vector2 anchorMin = oldRect.anchorMin;
        Vector2 anchorMax = oldRect.anchorMax;
        Vector2 anchoredPosition = oldRect.anchoredPosition;
        Vector2 sizeDelta = oldRect.sizeDelta;
        Vector2 pivot = oldRect.pivot;
        Quaternion localRotation = oldRect.localRotation;

        Undo.DestroyObjectImmediate(oldButton.gameObject);

        GameObject newButtonObject = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, parent);
        Undo.RegisterCreatedObjectUndo(newButtonObject, "Create Bloodlines Button");
        newButtonObject.name = childName;
        newButtonObject.transform.SetSiblingIndex(siblingIndex);

        RectTransform newRect = newButtonObject.GetComponent<RectTransform>();
        newRect.anchorMin = anchorMin;
        newRect.anchorMax = anchorMax;
        newRect.anchoredPosition = anchoredPosition;
        newRect.sizeDelta = new Vector2(Mathf.Max(sizeDelta.x, MinButtonWidth), ButtonHeight);
        newRect.pivot = pivot;
        newRect.localRotation = localRotation;
        newRect.localScale = Vector3.one;

        SetLabel(newButtonObject.transform, label);
    }

    private static bool IsBloodlinesButton(GameObject gameObject)
    {
        return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject) == ButtonPrefabPath;
    }

    private static void SetLabel(Transform button, string label)
    {
        TextMeshProUGUI labelText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (labelText == null)
            return;

        Undo.RecordObject(labelText, "Set Button Label");
        labelText.text = label;
        EditorUtility.SetDirty(labelText);
    }
}
#endif
