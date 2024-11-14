using Paps.UnityToolbarExtenderUIToolkit;
using Toolbar.Editor;
using UnityEngine;
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
           ToolCollection.OnChanged += OnToolbarTypeChanged;
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

        public void Hide()
        {
            Debug.Log($"hiding");
            style.display = DisplayStyle.None;
        }

        public void Show()
        {
            Debug.Log($"showing");
            style.display = DisplayStyle.Flex;
        }
    }
}