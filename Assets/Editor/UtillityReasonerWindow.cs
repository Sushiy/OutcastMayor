using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class UtillityReasonerWindow : EditorWindow
{
    [MenuItem("Tools/Outcast Mayor/UtillityReasonerWindow")]
    public static void ShowExample()
    {
        UtillityReasonerWindow wnd = GetWindow<UtillityReasonerWindow>();
        wnd.titleContent = new GUIContent("UtillityReasonerWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UtillityReasonerWindow.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UtillityReasonerWindow.uss");
        root.styleSheets.Add(styleSheet);
    }
}