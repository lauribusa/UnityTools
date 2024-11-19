using System.IO;
using UnityEditor;
using UnityEngine;

namespace _.Features.Symlink.Editor
{
    public class SymlinkWindow: EditorWindow
    {
        #region Public
        #endregion
        
        #region Private & Protected
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
            foreach (var directory in directoryInfo!.GetDirectories())
            {
                Debug.Log($"{directory.FullName}");
            }
        }
        #endregion
    }
}