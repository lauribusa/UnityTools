using Paps.UnityToolbarExtenderUIToolkit;
using SceneLoader.Data;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesLoader", order: 1)]
    public class SceneDataField : ObjectField
    {
        [AssetReferenceUILabelRestriction("sceneData")]
        private AssetReference assetReference;

        [Serialize]
        public SceneData SceneData;
        public void InitializeElement()
        {
            label = "Load:";
            objectType = typeof(AssetReference);
            RegisterCallback<ChangeEvent<AssetReference>>(OnObjectFieldValueChanged);
        }
        private void OnObjectFieldValueChanged(ChangeEvent<AssetReference> evt)
        {
            if (evt.newValue is not null)
            {
                var handle = evt.newValue.LoadAssetAsync<SceneData>();
                handle.Completed += OnLoadingComplete;
            }
            else
            {
                SceneLoaderEditor.UnloadSceneData();
            }
        }

        private void OnLoadingComplete(AsyncOperationHandle<SceneData> handle)
        {
            var sceneData = handle.Result;
            SceneLoaderEditor.LoadSceneData(sceneData);
            SceneData = sceneData;
        }
    }
}
