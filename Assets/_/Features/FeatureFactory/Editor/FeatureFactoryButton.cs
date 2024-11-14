using Paps.UnityToolbarExtenderUIToolkit;
using Toolbar.Editor;
using UnityEngine.UIElements;

namespace FeatureFactory.Editor
{
    [MainToolbarElement("FeatureCreator", ToolbarAlign.Right)]
    public class FeatureFactoryButton: Button, IToolbarElement
    {
        private DisplayStyle _displayStyle;

        public void InitializeElement()
        {
            text = "Add Feature";
            clicked += OnClick;
            ToolCollection.OnChanged += OnToolbarTypeChanged;
        }

        private void OnClick()
        {
            FeatureFactoryEditor.ShowWindow();
        }

        public DisplayStyle DisplayStyle { get; set; }
        public string Type { get; set; } = ToolType.FEATURES;
        public void OnToolbarTypeChanged(string value)
        {
            style.display = value == Type ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
