using Paps.UnityToolbarExtenderUIToolkit;
using SceneLoader.Data;
using UnityEditor;
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
        
        public string Path { get; private set; }
        public void InitializeElement()
        {
            label = "Load:";
            objectType = typeof(SceneData);
            RegisterCallback<ChangeEvent<SceneData>>(OnObjectFieldValueChanged);
        }
        private void OnObjectFieldValueChanged(ChangeEvent<SceneData> evt)
        {
            if (evt.newValue is not null)
            {
                var sceneData = evt.newValue;
                Path = AssetDatabase.GetAssetPath(sceneData);
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
