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
        public void InitializeElement()
        {
            label = "Load:";
            objectType = typeof(SceneData);
            this.RegisterValueChangedCallback(OnObjectFieldValueChanged);
        }
        private void OnObjectFieldValueChanged(ChangeEvent<Object> evt)
        {
            if (evt.newValue is SceneData sceneData)
            {
                LoadSceneData(sceneData);
            }
            else
            {
                SceneLoaderEditor.UnloadSceneEditor();
            }
        }

        private void LoadSceneData(SceneData sceneData)
        {
            SceneLoaderEditor.LoadSceneEditor(sceneData);
        }
    }
}
