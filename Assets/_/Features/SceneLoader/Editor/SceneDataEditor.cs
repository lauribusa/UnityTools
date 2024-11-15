using System.Collections.Generic;
using SceneLoader.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Scene Data", GUILayout.MaxWidth(100f));
            _sceneName = GUILayout.TextField(_sceneName);
            GUILayout.EndHorizontal();
            if (!GUILayout.Button(new GUIContent("Create Scene Data"))) return;
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
            var composedPath = $@"{PATH}\{sceneName}.{UNITY}";
            EditorSceneManager.SaveScene(scene, composedPath);
            EditorSceneManager.CloseScene(scene, true);
            var sceneGuid = AssetDatabase.AssetPathToGUID(composedPath);
            var assetReference = new AssetReference(sceneGuid);
            List<AssetReference> referenceList = new(_loadedData.sceneAssetReferences) { assetReference };
            _loadedData.sceneAssetReferences = referenceList.ToArray();
            Debug.Log($"Create scene {sceneName}");
        }

        public static void LoadSceneData(SceneData sceneData)
        {
            UnloadSceneData();
            sceneData.LoadScenesEditor();
            _loadedData = sceneData;
        }

        public static void UnloadSceneData()
        {
            if(_loadedData != null)
            {
                _loadedData.CloseSceneEditor();
            }
        }

        private static readonly string PATH = $@"Assets\_\Levels";
        private const string UNITY = "unity";
    }
}
