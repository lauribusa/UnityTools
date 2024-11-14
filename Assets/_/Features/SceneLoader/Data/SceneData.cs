using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SceneLoader.Data
{
    [CreateAssetMenu(menuName = "Game/SceneData")]
    public class SceneData : ScriptableObject
    {
        #region Publics
        public int[] sceneIndices;
        public AssetReference[] scenes;
        private List<Scene> _activeScenesInEditor = new();
        private List<SceneInstance> _instances = new();

        #endregion

        #region Public API

        public bool IsSceneValid(GUID guid)
        {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].guid == guid) return true;
            }
            Debug.LogError($"ERROR: No valid scene found for guid {guid}. Perhaps missing from the build settings?");
            return false;
        }
        public void LoadScenesEditor()
        {
            for (int i = 0; i < sceneIndices.Length; i++)
            {

                var scenePath = EditorBuildSettings.scenes[i].path;
                if (!IsSceneValid(AssetDatabase.GUIDFromAssetPath(scenePath))) continue;
                if (sceneIndices[i] == 0)
                {
                    _activeScenesInEditor.Add(EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single));
                    continue;
                }
                _activeScenesInEditor.Add(EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive));
            }
        }

        public void CloseSceneEditor()
        {
            var count = _activeScenesInEditor.Count;
            for (int i = count-1; i > 0; i--)
            {
                var scene = _activeScenesInEditor[i];
                EditorSceneManager.CloseScene(scene, true);
                _activeScenesInEditor.Remove(scene);
            }
        }

        #endregion

        public async void LoadScenes()
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                SceneInstance result;
                if (i == 0)
                {
                    var loadHandle = Addressables.LoadSceneAsync(scenes[i], LoadSceneMode.Single);
                    result = await loadHandle.Task;
                    AddToLoadedScenes(result);
                    continue;
                }
                var handle = Addressables.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);
                result = await handle.Task;
                AddToLoadedScenes(result);
            }
        }

        private void AddToLoadedScenes(SceneInstance scene)
        {
            _instances.Add(scene);
        }

        public async void CloseScene()
        {
            var count = _instances.Count;
            for (int i = count - 1; i > 0; i--)
            {
                var result = await Addressables.UnloadSceneAsync(_instances[i]).Task;
                RemoveFromLoadedScenes(result);
            }
        }

        private void RemoveFromLoadedScenes(SceneInstance scene)
        {
            _instances.Remove(scene);
        }
    }
}
