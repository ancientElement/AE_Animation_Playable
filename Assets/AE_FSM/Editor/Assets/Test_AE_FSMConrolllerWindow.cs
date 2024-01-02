using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Test_AE_FSMConrolllerWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/Test_AE_FSMConrolllerWindow")]
    public static void ShowExample()
    {
        Test_AE_FSMConrolllerWindow wnd = GetWindow<Test_AE_FSMConrolllerWindow>();
        wnd.titleContent = new GUIContent("Test_AE_FSMConrolllerWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AE_FSM/Editor/Assets/Test_AE_FSMConrolllerWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
    }
}