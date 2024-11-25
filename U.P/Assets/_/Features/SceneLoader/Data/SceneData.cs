using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace SceneLoader.Data
{
    [CreateAssetMenu(menuName = "Game/SceneData")]
    public class SceneData : ScriptableObject
    {
        #region Variables
        
        public AssetReference[] sceneAssetReferences;
        private List<SceneInstance> _instances = new();
        #endregion

        #region Public API
        public void LoadScenes()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                LoadScenesRuntime();
                return;
            }
            LoadScenesEditor();
            #else
            LoadScenesRuntime();
            #endif
        }
        #endregion
        
        #region Editor Functions
        public void LoadScenesEditor()
        {
            #if UNITY_EDITOR
            for (int i = 0; i < sceneAssetReferences.Length; i++)
            {
                if (!IsSceneValid(sceneAssetReferences[i], out var path)) continue;
                EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }
            #endif
        }
        
        public void CloseSceneEditor()
        {
            #if UNITY_EDITOR
            for (int i = 0; i < sceneAssetReferences.Length; i++)
            {
                if (!IsSceneValid(sceneAssetReferences[i], out var path)) continue;
                var scene = EditorSceneManager.GetSceneByPath(path);
                EditorSceneManager.CloseScene(scene, true);
            }
            #endif
        }
        #endregion
        
        #region Runtime Functions
        public async void LoadScenesRuntime()
        {
            for (var index = 0; index < sceneAssetReferences.Length; index++)
            {
                var asset = sceneAssetReferences[index];
                var handle = Addressables.LoadSceneAsync(asset, LoadSceneMode.Additive);
                var result = await handle.Task;
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
        private bool IsSceneValid(AssetReference assetReference, out string path)
        {
            path = "";
            #if UNITY_EDITOR
            path = AssetDatabase.GUIDToAssetPath(assetReference.AssetGUID);
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path == path) return true;
            }
            Debug.LogError($"ERROR: No valid scene found for path {path}. Perhaps missing from the build settings?");
            #endif
            return false;
        }    

        #endregion
    }
}
