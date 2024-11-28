using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesLoaderRuntimeOverride", order: 0)]
    public class SceneDataLoaderRuntimeOverride: Toggle
    {
        public static SceneDataLoaderRuntimeOverride Instance { get; private set; }
        [Serialize]
        public bool _overrideRuntime;
        public void InitializeElement()
        {
            value = _overrideRuntime;
            label = "Runtime Override";
            RegisterCallback<ChangeEvent<bool>>(OnChange);
            Instance = this;
        }

        private void OnChange(ChangeEvent<bool> evt)
        {
            _overrideRuntime = evt.newValue;
        }
    }
}