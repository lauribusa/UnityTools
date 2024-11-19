using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SceneLoader.Data;
using SceneLoader.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using OnOpenAsset = UnityEditor.Callbacks.OnOpenAssetAttribute;

namespace SceneLoader.Editor
{
    [CustomEditor(typeof(SceneData))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        [SerializeField] private string _sceneName;
        private static SceneData _loadedData;
        [OnOpenAsset]
        public static bool OnDoubleClick(int instanceId, int line, int row)
        {
            var target = EditorUtility.InstanceIDToObject(instanceId);
            switch (target)
            {
                case SceneData sceneData:
                    LoadSceneData(sceneData);
                    return true;
                default:
                    return false;
            }
        }

        public override VisualElement CreateInspectorGUI()
        {

            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            var buttonGroup = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            var textField = new TextField(_sceneName) { label= "Scene Name", style = { flexGrow = 1} };
            var createButton = new Button(OnCreateSceneEvent) { text = "Create Scene" };
            var deleteButton = new Button(OnDeleteSceneEvent) { text = "Delete Scene" };
            textField.RegisterCallback<ChangeEvent<string>>(OnTextChange);
            buttonGroup.Add(textField);
            root.Add(buttonGroup);
            root.Add(createButton);
            root.Add(deleteButton);
            return root;
        }

        private void OnTextChange(ChangeEvent<string> evt)
        {
            _sceneName = evt.newValue;
        }
        
        private void OnDeleteSceneEvent()
        {
            if (EditorUtility.DisplayDialog("Delete Scene", $"Do you wish to delete the last scene?", "OK",
                    "Cancel"))
            {
                DeleteScene();
            }
        }

        private void OnCreateSceneEvent()
        {
            if (FileAlreadyExists(_sceneName))
            {
                EditorUtility.DisplayDialog("Error", $@"A file at path {PATH}\{_sceneName}.{UNITY}\ already exists.", "OK");
                return;
            }

            if (!IsValidFilename(_sceneName) || string.IsNullOrWhiteSpace(_sceneName))
            {
                EditorUtility.DisplayDialog("Error", $"The name is empty or contains invalid characters.", "OK");
                return;
            }
            if (EditorUtility.DisplayDialog("Create Scene", $"Do you wish to create the scene {_sceneName}?", "OK",
                    "Cancel"))
            {
                CreateScene(_sceneName);
            }
        }

        private void CreateScene(string sceneName)
        {
            var sceneData = serializedObject.targetObject as SceneData;
            if (sceneData == null) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            var composedPath = $@"{PATH}\{sceneData.name}_{sceneName}.{UNITY}";
            EditorSceneManager.SaveScene(scene, composedPath);
            EditorSceneManager.CloseScene(scene, true);
            var sceneGuid = AssetDatabase.AssetPathToGUID(composedPath);
            var assetReference = new AssetReference(sceneGuid);
            List<AssetReference> referenceList = new(sceneData.sceneAssetReferences) { assetReference };
            sceneData.sceneAssetReferences = referenceList.ToArray();
            _sceneName = string.Empty;
        }

        private void DeleteScene()
        {
            var sceneData = serializedObject.targetObject as SceneData;
            if (sceneData == null) return;
            if(sceneData.sceneAssetReferences is not { Length: > 0 }) return;
            var assetReferences = new List<AssetReference>(sceneData.sceneAssetReferences);
            var lastScene = sceneData.sceneAssetReferences[^1];
            assetReferences.RemoveAt(assetReferences.Count - 1);
            var path = AssetDatabase.GUIDToAssetPath(lastScene.AssetGUID);
            if (!AssetDatabase.DeleteAsset(path))
            {
                EditorUtility.DisplayDialog("Error", $"Failed to delete asset at {path}", "OK");
                return;
            }
            sceneData.sceneAssetReferences = assetReferences.ToArray();
        }

        public static void LoadSceneData(SceneData sceneData)
        {
            UnloadSceneData();
            sceneData.LoadScenes();
            _loadedData = sceneData;
            //SceneDataState.Instance.SetData(sceneData);
        }

        public static void UnloadSceneData()
        {
            if(_loadedData != null)
            {
                _loadedData.CloseSceneEditor();
            }
        }
        
        private static bool FileAlreadyExists(string fileName)
        {
            if (!File.Exists($@"{PATH}\{fileName}.{UNITY}")) return false;
            EditorUtility.DisplayDialog("Error", $@"File at path {PATH}\{fileName}.{UNITY} already exists.", "OK");
            return true;
        }

        private static bool IsValidFilename(string testName)
        {
            var containsABadCharacter = new Regex("["+ Regex.Escape(new string(Path.GetInvalidFileNameChars())) +"]");
            return !containsABadCharacter.IsMatch(testName);
        }

        private static readonly string PATH = $@"Assets\_\Levels";
        private const string UNITY = "unity";
    }
}
