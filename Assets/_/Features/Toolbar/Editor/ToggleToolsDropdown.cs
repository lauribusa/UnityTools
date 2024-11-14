using System;
using System.Collections.Generic;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

namespace Toolbar.Editor
{
    [MainToolbarElement("ToolsDropdown", order: -1)]
    public class ToggleToolsDropdown: DropdownField
    {
        private static readonly string[] ELEMENTS = ToolType.All;
        [Serialize]
        private static string _selectedTool = ELEMENTS[0];

        public async void InitializeElement()
        {
            style.minWidth = 150;
            choices = new List<string>(ELEMENTS);
            value = _selectedTool;
            label = "Tools";
            RegisterCallback<ChangeEvent<string>>(OnChange);
            await System.Threading.Tasks.Task.Delay(100);
            ToolCollection.OnChanged?.Invoke(_selectedTool);
        }

        private void OnChange(ChangeEvent<string> evt)
        {
            _selectedTool = evt.newValue;
            ToolCollection.OnChanged.Invoke(evt.newValue);
        }
    }

    public static class ToolType
    {
        public const string ADDRESSABLE = "Addressables";
        public const string SCENE_DATA = "SceneData";
        public const string FEATURES = "Features";
        public static readonly string[] All = { ADDRESSABLE, SCENE_DATA, FEATURES };
    }

    public static class ToolCollection
    {
        private static readonly List<IToolbarElement> TOOLBAR_ELEMENTS = new();
        private static string _selectedTool = ToolType.All[0];
        public static Action<string> OnChanged;
    }

    public interface IToolbarElement
    {
        public DisplayStyle DisplayStyle { get; set; }
        public string Type { get; set; }

        public void OnToolbarTypeChanged(string value);
    }
}