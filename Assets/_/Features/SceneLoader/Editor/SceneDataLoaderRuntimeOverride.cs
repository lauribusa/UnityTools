using Paps.UnityToolbarExtenderUIToolkit;
using Toolbar.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesLoaderRuntimeOverride", order: 0)]
    public class SceneDataLoaderRuntimeOverride: Toggle
    {
        [Serialize] 
        private bool _boolValue;
        public void InitializeElement()
        {
            value = _boolValue;
            label = "Runtime Override";
            RegisterCallback<ChangeEvent<bool>>(OnChange);
        }

        private void OnChange(ChangeEvent<bool> evt)
        {
            Debug.Log($"{label}: {evt.newValue}");
            _boolValue = evt.newValue;
        }
    }
}