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
        public bool IsActive { get; private set; }
        public string FullPath => name;
        public ToggleFolder(string directoryName, string unicodeSymbol = "", string parentPath = null)
        {
            name = parentPath != null ? $@"{parentPath}\{directoryName}" : directoryName;
            if (!string.IsNullOrWhiteSpace(unicodeSymbol))
            {
                Add(new Label($"{unicodeSymbol}"));
            }
            var toggle = new Toggle
            {
                style =
                {
                    marginRight = 20
                }
            };
            toggle.RegisterCallback<ChangeEvent<bool>>(OnToggle);
            style.flexDirection = FlexDirection.Row;
            Add(toggle);
            Add(new Label($"{directoryName}"));
        }

        private void OnToggle(ChangeEvent<bool> evt)
        {
            IsActive = evt.newValue;
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
            }
        }

        public Tuple<string, bool>[] GetActivatedFoldersAndSubfolders()
        {
            var allFolders = new List<ToggleFolder>();
            allFolders.AddRange(_folders);
            foreach (var subFolderGroup in _subFoldersGroup)
            {
                var subFolders = subFolderGroup._folders.ToArray();
                allFolders.AddRange(subFolders);
            }
            var activatedFoldersAndSubfolders = new Tuple<string, bool>[allFolders.Count];
            for (int i = 0; i < activatedFoldersAndSubfolders.Length; i++)
            {
                activatedFoldersAndSubfolders[i] = new Tuple<string, bool>(allFolders[i].name, allFolders[i].IsActive);
            }
            return activatedFoldersAndSubfolders;
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