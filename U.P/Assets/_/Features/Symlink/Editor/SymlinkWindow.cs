using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Features.Symlink.Editor
{
    public class SymlinkWindow : EditorWindow
    {
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
        #endregion
        // TODO: IGNORE ROOT FOLDERS AND ONLY CHECK FOR SUBFOLDERS IN GRAPHICS/AUDIO/LEVELS
        #region Main
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
            for (var index = 0; index < folders.Length; index++)
            {
                var folder = folders[index];
                var linkDirectory = new DirectoryInfo($@"{LinkPath}\{folder.Name}");
                if (folder.IsToggled)
                {
                    if (linkDirectory.Exists)
                    {
                        if (!DirectoryIsSymlink(linkDirectory)) DeleteDirectory(linkDirectory.FullName);
                        continue;
                    }
                    CreateSymlinkDirectory(folder.Name);
                    continue;
                }
    
                if (!linkDirectory.Exists) continue;
                if (!DirectoryIsSymlink(linkDirectory)) continue;

                DeleteDirectory(linkDirectory.FullName);
            }
        }

        private void CheckForExistingSymlinks()
        {
            var folders = _rootFolderGroup.GetFoldersAndSubfolders();
            for (var index = 0; index < folders.Length; index++)
            {
                var folder = folders[index];
                var targetDirectory = new DirectoryInfo($@"{LinkPath}\{folder.Name}");
                var isValidSymlink = targetDirectory.Exists && DirectoryIsSymlink(targetDirectory);
                if (!isValidSymlink) continue;
                folder.SetToggleState(true);
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

        private void DeleteDirectory(string path)
        {
            Directory.Delete(path);
            File.Delete($"{path}.meta");
            AssetDatabase.Refresh();
        }
        private void CreateSymlinkDirectory(string path)
        {
            var originPath = $@"{OriginPath}\{path}";
            var originDirectory = new DirectoryInfo(originPath);
            
            var targetPath = $@"{LinkPath}\{path}";
            var targetDirectory = new DirectoryInfo(targetPath);
            
            Directory.CreateDirectory(Directory.GetParent(targetDirectory.FullName).FullName);
            
            var subFolderPaths = path.Split('\\');
            var nestedPath = "";
            for (int i = 0; i < subFolderPaths.Length - 1; i++)
            {
                var subFolder = subFolderPaths[i];
                nestedPath += $@"\{subFolder}";
                var nestedDirectory = new DirectoryInfo($@"{LinkPath}{nestedPath}");
                if (nestedDirectory.Exists) continue;
                Directory.CreateDirectory($"{LinkPath}{nestedPath}");
            }
            if (targetDirectory.Exists)
            {
                if (DirectoryIsSymlink(targetDirectory))
                {
                    Debug.LogWarning($"Symlink directory already exists: {targetDirectory.FullName}");
                    return;
                }

                Debug.LogError($"A non-symlink directory already exists at path {targetDirectory.FullName}");
                return;
            }

            if (!originDirectory.Exists)
            {
                Debug.LogError($"Could not found directory at path {originDirectory.FullName}");
                return;
            }
            var success = MakeSymlinkFromPowerShell(targetPath, originPath);

            Debug.Log(success
                ? $"Created Symlink Directory at {targetPath} from {originPath}"
                : $"Failed to create Symlink Directory at {targetPath} from {originPath}.");
        }
        
        private bool MakeSymlinkFromCommandLine(string linkPath, string originPath)
        {
            var args = $"/c mklink /J {linkPath} {originPath}";
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = $"cmd.exe",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Arguments = args
                }
            };
            return RunProcess(process);
        }

        private bool MakeSymlinkFromPowerShell(string linkPath, string originPath)
        {
            var args = $"New-Item -Path {linkPath} -ItemType Junction -Value {originPath}";
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = $"powershell.exe",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Arguments = args
                }
            };
            return RunProcess(process);
        }

        private static bool RunProcess(System.Diagnostics.Process process)
        {
            process.Start();
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            if(stdout.Length > 0)Debug.Log($"STDOUT: {stdout}");
            if(stderr.Length > 0)Debug.LogError($"STDERR: {stderr}");
            process.WaitForExit();
            var exitCode = process.ExitCode;
            Debug.unityLogger.logEnabled = false;
            AssetDatabase.Refresh();
            Debug.unityLogger.logEnabled = true;
            process.Dispose();
            return exitCode == 0;
        }
        #endregion
    }
}