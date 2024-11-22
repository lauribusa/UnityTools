using SceneLoader.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SceneLoader.Runtime
{
    public class SceneDataState : MonoBehaviour
    {
        #region Private & Protected
        [SerializeField]
        private AssetReferenceSceneData sceneData;
        #endregion

        #region Unity API

        private void Awake()
        {
            var runtimeOverride = SessionState.GetBool("RuntimeOverride", false);
            var assetRef = sceneData;
            if (runtimeOverride)
            {
                var sceneDataPath = SessionState.GetString("SceneDataPath", string.Empty);
                var guid = AssetDatabase.AssetPathToGUID(sceneDataPath);
                var assetReference = new AssetReferenceSceneData(guid);
                assetRef = assetReference;
            }
            if(assetRef == null)
            {
                throw new System.Exception($"No scene set loaded in scene loader.");
            }
            var handler = Addressables.LoadAssetAsync<SceneData>(assetRef);
            handler.Completed += LoadSceneData;
        }

        private void LoadSceneData(AsyncOperationHandle<SceneData> handler)
        {
            handler.Result.LoadScenes();
        }
        #endregion
        
        
    }
}
