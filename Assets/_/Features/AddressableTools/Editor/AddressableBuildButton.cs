using Paps.UnityToolbarExtenderUIToolkit;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.UIElements;

namespace AddressableTools.Editor
{
    [MainToolbarElement("AddressableBuild", ToolbarAlign.Right, order: 0)]
    public class AddressableBuildButton: Button
    {
        public void InitializeElement()
        {
            text = "Addressable Build";
            clicked += OnClick;
        }

        private void OnClick()
        {
            AddressableAssetSettings.BuildPlayerContent();
        }
    }
}