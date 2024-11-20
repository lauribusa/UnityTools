using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Features.Symlink.Editor
{
    public class ToggleFolder : VisualElement
    {
        public string Name => name;
        private readonly Toggle _toggle;
        public bool IsToggled => _toggle.value;
        private ToggleFolderGroup _subfolders;
        public ToggleFolder(string directoryName, string unicodeSymbol = "", string parentPath = null)
        {
            name = parentPath != null ? $@"{parentPath}\{directoryName}" : directoryName;
            if (!string.IsNullOrWhiteSpace(unicodeSymbol))
            {
                Add(new Label($"{unicodeSymbol}"));
            }
            _toggle = new Toggle
            {
                style =
                {
                    marginRight = 20
                }
            };
            _toggle.RegisterValueChangedCallback(OnToggle);
            style.flexDirection = FlexDirection.Row;
            style.backgroundColor = new StyleColor(Color.blue);
            Add(_toggle);
            Add(new Label($"{directoryName}"));
        }

        public void HideToggle()
        {
            // make disabled
            _toggle.style.display = DisplayStyle.None;
        }

        public void ShowToggle()
        {
            _toggle.style.display = DisplayStyle.Flex;
        }

        private void OnToggle(ChangeEvent<bool> evt)
        {
            if (_subfolders == null) return;
            var folders = _subfolders.GetFoldersAndSubfolders();
            if (!evt.newValue)
            {
                for (int i = 0; i < folders.Length; i++)
                {
                    folders[i].ShowToggle();
                }
                return;
            }
            for (int i = 0; i < folders.Length; i++)
            {
                var subfolder = folders[i];
                subfolder.SetToggleState(false);
                subfolder.HideToggle();
            }
        }

        public void SetSubfolders(ToggleFolderGroup subfolders)
        {
            _subfolders = subfolders;
        }

        public void SetToggleState(bool value)
        {
            _toggle.value = value;
        }
    }

    public class ToggleFolderGroup : VisualElement
    {
        private const string PATTERN = @"^[.]\w+";
        private readonly List<ToggleFolder> _folders = new();
        private List<ToggleFolderGroup> _subFoldersGroup = new();
        private string parentPath = string.Empty;
        public ToggleFolderGroup(DirectoryInfo parent, ToggleFolderGroup root = null, string parentPath = null)
        {
            style.marginLeft = 15;
            style.backgroundColor = new StyleColor(Color.red);
            //if(root == null) Add(new Label($"{parent.Name}"));
            foreach (var directory in parent.GetDirectories())
            {
                var subFolderPath = parentPath != null ? $@"{parentPath}\{directory.Name}" : directory.Name;
                var match = Regex.Match(@directory.Name, PATTERN);
                if (match.Success) continue;
                var directoryElement = new ToggleFolder(directory.Name, "\u2514", parentPath);
                Add(directoryElement);
                _folders.Add(directoryElement);
                var subDirectoryInfo = new DirectoryInfo(Path.Combine(parent.FullName, directory.Name));
                if (subDirectoryInfo.GetDirectories().Length <= 0) continue;
                var subFolderGroup = new ToggleFolderGroup(subDirectoryInfo, this, subFolderPath);
                Add(subFolderGroup);
                _subFoldersGroup.Add(subFolderGroup);
                directoryElement.SetSubfolders(subFolderGroup);
                // assign subfolders to a togglegroup and make all childrens' toggle hidden and value set to false.
                // if a toggle folder is toggled on, look at all children folders and disable them.
            }
        }

        /// <summary>
        /// Will return a ToggleFolder from within its elements and sub-elements with the specified name.
        /// </summary>
        /// <param name="folderName">The name of the ToggleFolder to find</param>
        /// <returns>Returns null if none are found.</returns>
        public ToggleFolder GetFolder(string folderName)
        {
            var allFolders = new List<ToggleFolder>();
            allFolders.AddRange(_folders);
            for (var index = 0; index < _subFoldersGroup.Count; index++)
            {
                var subFolderGroup = _subFoldersGroup[index];
                var subFolders = subFolderGroup._folders;
                allFolders.AddRange(subFolders);
            }

            for (var index = 0; index < allFolders.Count; index++)
            {
                if (allFolders[index].name != folderName) continue;
                return allFolders[index];
            }

            return null;
        }

        public ToggleFolder[] GetFoldersAndSubfolders()
        {
            var allFolders = new List<ToggleFolder>();
            allFolders.AddRange(_folders);
            for (var index = 0; index < _subFoldersGroup.Count; index++)
            {
                var subFolders = _subFoldersGroup[index]._folders.ToArray();
                allFolders.AddRange(subFolders);
            }

            return allFolders.ToArray();
        }
    }

    public class ButtonConfirmCancelGroup: VisualElement
    {
        public ButtonConfirmCancelGroup(Action confirmAction, Action cancelAction = null)
        {
            style.flexDirection = FlexDirection.Row;
            style.marginLeft = 10;
            style.marginTop = 20;
            var applyButton = new Button(confirmAction) { text = "Confirm" };
            Add(applyButton);
            if (cancelAction == null) return;
            var cancelButton = new Button(cancelAction) { text = "Cancel" };
            Add(cancelButton);
        }
    }
}