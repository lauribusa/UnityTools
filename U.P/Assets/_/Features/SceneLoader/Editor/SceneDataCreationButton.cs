using Paps.UnityToolbarExtenderUIToolkit;
using Toolbar.Editor;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesCreator", ToolbarAlign.Right, order: 2)]
    public class SceneDataCreationButton: Button, IToolbarElement
    {
        public void InitializeElement()
        { 
           text = "ScenesCreator";
           clicked += OnClick;
           style.display = DisplayStyle;
           ToolEvent.OnChanged += OnToolbarTypeChanged;
        }

        private static void OnClick()
        {
            SceneDataCreationWindow.ShowWindow();
        }

        [Serialize]
        public DisplayStyle DisplayStyle { get; set; }
        
        public string Type { get; set; } = ToolType.SCENE_DATA;
        public void OnToolbarTypeChanged(string value)
        {
            style.display = value == Type ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}