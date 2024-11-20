using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Features.Symlink.Editor
{
    public class ToggleFolder : VisualElement
    {
        public bool IsActive { get; private set; }
        public string Name => name;
        private readonly Toggle _toggle;
        public bool IsToggled => _toggle.value;
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
            _toggle.RegisterCallback<ChangeEvent<bool>>(OnToggle);
            style.flexDirection = FlexDirection.Row;
            Add(_toggle);
            Add(new Label($"{directoryName}"));
        }

        private void OnToggle(ChangeEvent<bool> evt)
        {
            IsActive = evt.newValue;
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
        public ToggleFolderGroup Parent { get; private set; }
        public ToggleFolderGroup(DirectoryInfo parent, ToggleFolderGroup root = null, string parentPath = null)
        {
            style.marginLeft = 15;
            foreach (var directory in parent.GetDirectories())
            {
                var subFolderPath = parentPath != null ? $@"{parentPath}\{directory.Name}" : directory.Name;
                var match = Regex.Match(@directory.Name, PATTERN);
                if (match.Success) continue;
                var directoryElement = new ToggleFolder(directory.Name, "\u2514", parentPath);
                Add(directoryElement);
                _folders.Add(directoryElement);
                var subDirectoryInfo = new DirectoryInfo(Path.Combine(parent.FullName, directory.Name));
                var subFolderGroup = new ToggleFolderGroup(subDirectoryInfo, this, subFolderPath);
                Add(subFolderGroup);
                _subFoldersGroup.Add(subFolderGroup);
                if (root != null)
                {
                    Parent = root;
                }
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
            foreach (var subFolderGroup in _subFoldersGroup)
            {
                var subFolders = subFolderGroup._folders;
                allFolders.AddRange(subFolders);
            }

            foreach (var subFolder in allFolders)
            {
                if (subFolder.name != folderName) continue;
                return subFolder;
            }
            return null;
        }

        public ToggleFolder[] GetFoldersAndSubfolders()
        {
            var allFolders = new List<ToggleFolder>();
            allFolders.AddRange(_folders);
            foreach (var subFolderGroup in _subFoldersGroup)
            {
                var subFolders = subFolderGroup._folders.ToArray();
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