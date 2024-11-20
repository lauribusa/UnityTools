using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Features.Symlink.Editor
{
    public class SymlinkWindow : EditorWindow
    {
        #region Public

        #endregion

        #region Private & Protected

        private static string LinkPath => $@"{Directory.GetCurrentDirectory()}\Assets\_";
        private static string OriginPath => $@"{Directory.GetParent(Directory.GetCurrentDirectory())}\symlink";
        private ToggleFolderGroup _rootFolderGroup;

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
            DrawWindow();
        }

        private void DrawWindow()
        {
            var symlinkDirectory = new DirectoryInfo(OriginPath);
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
            rootVisualElement.Add(scrollView);
            scrollView.Add(label);
            _rootFolderGroup = new ToggleFolderGroup(symlinkDirectory);
            scrollView.Add(_rootFolderGroup);
            scrollView.Add(new ButtonConfirmCancelGroup(ConfirmSymlinkToggles, CancelSymlinkToggles));
            CheckForExistingSymlinks();
        }

        private void ConfirmSymlinkToggles()
        {
            var folders = _rootFolderGroup.GetFoldersAndSubfolders();
            foreach (var folder in folders)
            {
                var linkPath = $@"{LinkPath}\{folder.Name}";
                var linkDirectory = new DirectoryInfo(linkPath);
                if (folder.IsToggled)
                {
                    if (linkDirectory.Exists)
                    {
                        if (!DirectoryIsSymlink(linkDirectory))
                        {
                            Debug.LogError($"A non-symlink directory already exists at path {linkDirectory.FullName}");
                        }
                        continue;
                    }
                    CreateSymlinkDirectory(folder.Name);
                }
                else
                {
                    if (!linkDirectory.Exists)
                    {
                        continue;
                    }
                    if (!DirectoryIsSymlink(linkDirectory))
                    {
                        Debug.LogError($"Directory exists but is not symlink: {linkDirectory.FullName}");
                        continue;
                    }
                    DeleteSymlinkDirectory(folder.Name);
                }
            }
        }

        private void CheckForExistingSymlinks()
        {
            var folders = _rootFolderGroup.GetFoldersAndSubfolders();
            foreach (var folder in folders)
            {
                var targetDirectory = new DirectoryInfo($@"{LinkPath}\{folder.Name}");
                var isValidSymlink = targetDirectory.Exists && DirectoryIsSymlink(targetDirectory);
                if (!isValidSymlink) continue;
                var foundFolder = _rootFolderGroup.GetFolder(folder.Name);
                if (foundFolder == null)
                {
                    Debug.LogError($"Couldn't find Symlink Folder with name: {folder.Name} in FolderGroup.");
                    continue;
                }

                foundFolder.SetToggleState(true);
            }
        }

        private void CancelSymlinkToggles()
        {
            rootVisualElement.Clear();
            DrawWindow();
        }

        private bool DirectoryIsSymlink(DirectoryInfo directoryInfo)
        {
            return directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        private void DeleteSymlinkDirectory(string path)
        {
            var linkPath = $@"{LinkPath}\{path}";
            var linkDirectory = new DirectoryInfo(linkPath);
            Debug.Log($"to delete: {linkDirectory.FullName}");
            Directory.Delete(linkPath);
        }
        private void CreateSymlinkDirectory(string path)
        {
            var linkPath = $@"{LinkPath}\{path}";
            var originPath = $@"{OriginPath}\{path}";
            var linkDirectory = new DirectoryInfo(linkPath);
            var originDirectory = new DirectoryInfo(originPath);
            var subFolders = path.Split('\\');
            var nestedPath = "";
            for (int i = 0; i < subFolders.Length - 1; i++)
            {
                var subFolder = subFolders[i];
                nestedPath += $@"\{subFolder}";
                var nestedDirectory = new DirectoryInfo(linkPath);
                if (nestedDirectory.Exists) continue;
                Directory.CreateDirectory($"{LinkPath}{nestedPath}");
            }
            if (linkDirectory.Exists)
            {
                if (DirectoryIsSymlink(linkDirectory))
                {
                    Debug.LogWarning($"Symlink directory already exists: {linkDirectory.FullName}");
                    return;
                }

                Debug.LogError($"A non-symlink directory already exists at path {linkDirectory.FullName}");
                return;
            }

            if (!originDirectory.Exists)
            {
                EditorUtility.DisplayDialog("Error",
                    $"Could not find a valid directory at path {originDirectory.FullName}", "OK");
                return;
            }
            var success = MakeSymlinkFromPowerShell(linkPath, originPath);

            Debug.Log(success
                ? $"Created Symlink Directory at {linkPath} from {originPath}"
                : $"Failed to create Symlink Directory at {linkPath} from {originPath}.");
        }
        
        private bool MakeSymlinkFromCommandLine(string linkPath, string originPath)
        {
            var args = $"/c mklink /J {linkPath} {originPath}";
            return RunProcess(args, "cmd");
        }

        private bool MakeSymlinkFromPowerShell(string linkPath, string originPath)
        {
            var args = $"New-Item -Path {linkPath} -ItemType Junction -Value {originPath}";
            return RunProcess(args, "powershell");
        }

        private static bool RunProcess(string args, string processName)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = $"{processName}.exe",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Arguments = args
                }
            };
            process.Start();
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            if(stdout.Length > 0)Debug.Log($"{stdout} || END.");
            if(stderr.Length > 0)Debug.LogError($"{stderr} || END.");
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        private void PrintAllSymlinkDir(DirectoryInfo directoryInfo)
        {
            if (DirectoryIsSymlink(directoryInfo))
            {
                Debug.Log($"Is Symlink: {directoryInfo.FullName}");
            }

            foreach (var dir in directoryInfo.GetDirectories())
            {
                PrintAllSymlinkDir(dir);
            }
        }
        #endregion
    }
}