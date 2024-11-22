using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesLoaderRuntimeOverride", order: 0)]
    public class SceneDataLoaderRuntimeOverride: Toggle
    {
        [Serialize]
        public bool _overrideRuntime;
        public void InitializeElement()
        {
            value = _overrideRuntime;
            label = "Runtime Override";
            RegisterCallback<ChangeEvent<bool>>(OnChange);
        }

        private void OnChange(ChangeEvent<bool> evt)
        {
            
            _overrideRuntime = evt.newValue;
        }
    }
}