using Paps.UnityToolbarExtenderUIToolkit;
using SceneLoader.Data;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesLoader", ToolbarAlign.Right)]
    public class ToolbarLevelField : ObjectField
    {
        public ToolbarLevelField(): base("Scene Loader:")
        {
            objectType = typeof(SceneData);
            var field = ElementAt(1);
            //PrintChildrens(field);
            // c'est de la merde ce package
            this.RegisterValueChangedCallback(OnObjectFieldValueChanged);
        }

        private void OnObjectFieldValueChanged(ChangeEvent<Object> evt)
        {
            Debug.Log($"{evt.newValue.name}");
        }

        private void PrintChildrens(VisualElement element)
        {
            foreach (var ve in element.Children())
            {
                Debug.Log(ve);
            }
        }
    }
}
