using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesCreator", ToolbarAlign.Right, order: 2)]
    public class SceneDataCreationButton: Button
    {
        public void InitializeElement()
        {
           text = "ScenesCreator";
           clicked += OnClick;
        }

        private void OnClick()
        {
            SceneDataCreationWindow.ShowWindow();
        }
    }
}