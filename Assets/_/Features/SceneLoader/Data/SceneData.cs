using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SceneLoader.Data
{
    [CreateAssetMenu(menuName = "Game/SceneData")]
    public class SceneData : ScriptableObject
    {
        #region Variables
        [FormerlySerializedAs("scenes")] public AssetReference[] sceneAssetReferences;
        private List<SceneInstance> _instances = new();

        #endregion

        #region Public API

        public void LoadScenes()
        {
            #if UNITY_EDITOR
            LoadScenesEditor();
            #else
            LoadScenesRuntime();
            #endif
        }
        
        #endregion
        
        #region Editor Functions
        public void LoadScenesEditor()
        {
            for (int i = 0; i < sceneAssetReferences.Length; i++)
            {
                if (!IsSceneValid(sceneAssetReferences[i], out var path)) continue;
                EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }
        }
        
        public void CloseSceneEditor()
        {
            for (int i = 0; i < sceneAssetReferences.Length; i++)
            {
                if (!IsSceneValid(sceneAssetReferences[i], out var path)) continue;
                var scene = EditorSceneManager.GetSceneByPath(path);
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        #endregion
        
        #region Runtime Functions
        public async void LoadScenesRuntime()
        {
            for (int i = 0; i < sceneAssetReferences.Length; i++)
            {
                SceneInstance result;
                if (i == 0)
                {
                    var loadHandle = Addressables.LoadSceneAsync(sceneAssetReferences[i], LoadSceneMode.Single);
                    result = await loadHandle.Task;
                    AddToLoadedScenesRuntime(result);
                    continue;
                }
                var handle = Addressables.LoadSceneAsync(sceneAssetReferences[i], LoadSceneMode.Additive);
                result = await handle.Task;
                AddToLoadedScenesRuntime(result);
            }
        }

        private void AddToLoadedScenesRuntime(SceneInstance scene)
        {
            _instances.Add(scene);
        }
        
        public async void CloseSceneRuntime()
        {
            var count = _instances.Count;
            for (int i = count - 1; i > 0; i--)
            {
                var result = await Addressables.UnloadSceneAsync(_instances[i]).Task;
                RemoveFromLoadedScenesRuntime(result);
            }
        }

        private void RemoveFromLoadedScenesRuntime(SceneInstance scene)
        {
            _instances.Remove(scene);
        }
        
        #endregion

        #region Private

        private bool IsSceneValid(AssetReference assetReference)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetReference.AssetGUID);
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path == path) return true;
            }
            Debug.LogError($"ERROR: No valid scene found for path {path}. Perhaps missing from the build settings?");
            return false;
        }
        
        private bool IsSceneValid(AssetReference assetReference, out string path)
        {
            path = AssetDatabase.GUIDToAssetPath(assetReference.AssetGUID);
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path == path) return true;
            }
            Debug.LogError($"ERROR: No valid scene found for path {path}. Perhaps missing from the build settings?");
            return false;
        }    

        #endregion
    }
}
