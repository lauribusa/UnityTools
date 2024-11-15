using Paps.UnityToolbarExtenderUIToolkit;
using Toolbar.Editor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.UIElements;

namespace AddressableTools.Editor
{
    [MainToolbarElement("AddressableBuild", ToolbarAlign.Right, order: 0)]
    public class AddressableBuildButton: Button, IToolbarElement
    {
        public void InitializeElement()
        {
            text = "Addressable Build";
            clicked += OnClick;
            ToolEvent.OnChanged += OnToolbarTypeChanged;
        }

        private void OnClick()
        {
            AddressableAssetSettings.BuildPlayerContent();
        }
        [Serialize]
        public DisplayStyle DisplayStyle { get; set; }
        public string Type { get; set; } = ToolType.ADDRESSABLE;
        public void OnToolbarTypeChanged(string value)
        {
            style.display = value == Type ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}