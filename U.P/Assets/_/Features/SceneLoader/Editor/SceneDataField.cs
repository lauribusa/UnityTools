using Paps.UnityToolbarExtenderUIToolkit;
using SceneLoader.Data;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesLoader", order: 1)]
    public class SceneDataField : ObjectField
    {
        [Serialize] 
        private SceneData _sceneData;
        public void InitializeElement()
        {
            label = "Load:";
            objectType = typeof(SceneData);
            RegisterCallback<ChangeEvent<Object>>(OnObjectFieldValueChanged);
        }
        private void OnObjectFieldValueChanged(ChangeEvent<Object> evt)
        {
            if (evt.newValue is SceneData sceneData)
            {
                LoadSceneData(sceneData);
            }
            else
            {
                SceneLoaderEditor.UnloadSceneData();
            }
        }

        private void LoadSceneData(SceneData sceneData)
        {
            SceneLoaderEditor.LoadSceneData(sceneData);
        }

        private void UnloadSceneData()
        {
            SceneLoaderEditor.UnloadSceneData();
        }
    }
}
