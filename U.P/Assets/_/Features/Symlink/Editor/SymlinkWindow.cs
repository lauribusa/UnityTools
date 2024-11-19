using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Features.Symlink.Editor
{
    public class SymlinkWindow: EditorWindow
    {
        #region Public
        #endregion
        
        #region Private & Protected
        private const string ORIGIN_PATH = "symlink";
        private const string LINK_PATH = @"\Assets\_";
        private ToggleFolderGroup _folderGroup;
        #endregion

        #region Unity API
        [MenuItem("Window/Symlink")]
        public static void ShowWindow()
        {
            var window = GetWindow<SymlinkWindow>();
            window.Show();
        }

        private void CreateGUI()
        {
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory());
            var symlinkDirectory = new DirectoryInfo(Path.Combine(directoryInfo!.FullName, ORIGIN_PATH));
            titleContent = new GUIContent("Symlink Editor");
            rootVisualElement.style.flexShrink = 1;
            var label = new Label("Directories:")
            {
                style =
                {
                    marginLeft = 5,
                    marginBottom = 10,
                    fontSize = 18
                }
            };
            var scrollView = new ScrollView
            {
                style =
                {
                    maxHeight = 500,
                }
            };
            var root = rootVisualElement;
            root.Add(scrollView);
            scrollView.Add(label);
            _folderGroup = new ToggleFolderGroup(symlinkDirectory);
            scrollView.Add(_folderGroup);
            scrollView.Add(new ButtonConfirmCancelGroup(ConfirmSymlinkToggles, CancelSymlinkToggles));
        }

        private void ConfirmSymlinkToggles()
        {
            var path = $@"{Directory.GetCurrentDirectory()}\{LINK_PATH}";
            var vals = _folderGroup.GetActivatedFoldersAndSubfolders();
            foreach (var v in vals)
            {
                var targetDirectory = new DirectoryInfo($@"{path}\{v.Item1}");
                Debug.Log($"{v.Item1} : {v.Item2} ({targetDirectory.FullName}) {targetDirectory.Exists}");
            }
        }

        private void CancelSymlinkToggles()
        {
            Debug.Log($"Canceled");
        }

        private void RunCommand(string linkPath, string targetPath)
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $@"mklink /J \{linkPath} \{targetPath}"
            };
            process.StartInfo = startInfo;
            process.Start();
        }
        #endregion
    }
}