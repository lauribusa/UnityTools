using System.IO;
using SceneLoader.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    public class SceneDataCreationWindow: EditorWindow
    {
        private bool _isFocused;
        private TextField _sceneDataName;
        private Button _confirm;
        public static void ShowWindow()
        {
            var wnd = GetWindow<SceneDataCreationWindow>();
            wnd.titleContent = new GUIContent("Scene Data Creation");
        }
        
        public void CreateGUI()
        {
            _isFocused = false;
            var root = rootVisualElement;
            _sceneDataName = new TextField("SceneData Name");
           
            _confirm = new Button(() =>
            {
                if (FileAlreadyExists(_sceneDataName.value)) return;
                CreateNewSceneData(_sceneDataName.value);
            })
            {
                text = "Create"
            };
            root.Add(_sceneDataName);
            root.Add(_confirm);
            Focus();
        }

        public void OnGUI()
        {
            if (_isFocused) return;
            _isFocused = true;
            _sceneDataName.Focus();
        }

        private void CreateNewSceneData(string sceneDataName)
        {
            var sceneData = CreateInstance<SceneData>();
            sceneData.name = sceneDataName;
            AssetDatabase.CreateAsset(sceneData, $@"{PATH}\{sceneDataName}.{ASSET}");
            AssetDatabase.SaveAssets();
        }
        
        private static bool FileAlreadyExists(string fileName)
        {
            if (!File.Exists($@"{PATH}\{fileName}.{ASSET}")) return false;
            EditorUtility.DisplayDialog("Error", $@"File at path {PATH}\{fileName}.{ASSET} already exists.", "OK");
            return true;
        }

        private const string PATH = @"Assets\_\Features\SceneLoader\Data";
        private const string ASSET = "asset";
    }
}