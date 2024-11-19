using UnityEditor;
using UnityEngine;

namespace SceneLoader.Editor
{
    public class SceneCreationWindow: EditorWindow
    {
        public static void ShowWindow()
        {
            var wnd = GetWindow<SceneCreationWindow>();
            wnd.titleContent = new GUIContent("Scene Creation");
        }
    }
}