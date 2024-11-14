using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

namespace FeatureFactory.Editor
{
    [MainToolbarElement("FeatureCreator", ToolbarAlign.Left)]
    public class FeatureFactoryButton: Button
    {
        public void InitializeElement()
        {
            text = "Add Feature";
            clicked += OnClick;
        }

        private void OnClick()
        {
            FeatureFactoryEditor.ShowWindow();
        }
    }
}
