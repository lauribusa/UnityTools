using System.IO;
using System.Text.RegularExpressions;
using _.Features.AddressableTools.Data;
using SceneLoader.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    public class SceneDataCreationWindow: EditorWindow
    {
        private bool _isFocused;
        private TextField _sceneDataName;
        private Button _confirm;
        [MenuItem("Assets/Create/SceneData", priority = 12)]
        public static void ShowWindow()
        {
            var wnd = GetWindow<SceneDataCreationWindow>();
            wnd.titleContent = new GUIContent("Scene Data Creation");
        }
        
        public void CreateGUI()
        {
            _isFocused = false;
            var root = rootVisualElement;
            _sceneDataName = new TextField("SceneData Name");
           
            _confirm = new Button(() =>
            {
                if (FileAlreadyExists(_sceneDataName.value)) return;
                if (!IsValidFilename(_sceneDataName.value)) return;
                if (string.IsNullOrWhiteSpace(_sceneDataName.value)) return; 
                    CreateNewSceneData(_sceneDataName.value);
            })
            {
                text = "Create"
            };
            root.Add(_sceneDataName);
            root.Add(_confirm);
            Focus();
        }

        public void OnGUI()
        {
            if (_isFocused || _sceneDataName == null) return;
            _isFocused = true;
            _sceneDataName.Focus();
        }

        private void CreateNewSceneData(string sceneDataName)
        {
            var sceneData = CreateInstance<SceneData>();
            var addressableDefinition = CreateInstance<AddressableDefinition>();
            sceneData.name = sceneDataName;
            var filePath = $@"{PATH}\{sceneDataName}\{sceneDataName}";
            var sceneDataPath = $"{filePath}.{ASSET}";
            var sceneFolderPath = $@"{PATH}\{sceneDataName}\scenes";
            Directory.CreateDirectory(sceneFolderPath);
            AssetDatabase.CreateAsset(sceneData, sceneDataPath);
            AssetDatabase.CreateAsset(addressableDefinition, $@"{filePath}.{nameof(AddressableDefinition)}.{ASSET}");
            AssetDatabase.SaveAssetIfDirty(sceneData);
            AssetDatabase.SaveAssetIfDirty(addressableDefinition);
            AssetDatabase.ImportAsset(sceneFolderPath);
            ShowNotification(new GUIContent("Created Asset."));
            System.Threading.Tasks.Task.Delay(100);
            EditorGUIUtility.PingObject(sceneData);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.FindGroup(sceneDataName) ?? settings.CreateGroup(sceneDataName, false, false, true, null);
            SetAsAddressable(sceneDataPath, group, true);
            AssetDatabase.SetLabels(addressableDefinition, new []{"ignore"});
            addressableDefinition.AddressableName = sceneDataName;
        }

        public static void SetAsAddressable(string path, bool clearLabels = false)
        {
            SetAsAddressable(path, AddressableAssetSettingsDefaultObject.Settings.DefaultGroup, clearLabels);
        }

        public static void SetAsAddressable(string path, AddressableAssetGroup group, bool clearLabels = false)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
            var addressableAssetEntry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
            addressableAssetEntry.address = path;
            if(clearLabels) addressableAssetEntry.labels.Clear();
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, addressableAssetEntry, false);
        }
        
        private static bool FileAlreadyExists(string fileName)
        {
            var filePath = $@"{PATH}\{fileName}\{fileName}.{ASSET}";
            if (!File.Exists(filePath)) return false;
            EditorUtility.DisplayDialog("Error", $@"File at path {filePath} already exists.", "OK");
            return true;
        }

        private static bool IsValidFilename(string testName)
        {
            var containsABadCharacter = new Regex("["+ Regex.Escape(new string(Path.GetInvalidFileNameChars())) +"]");
            return !containsABadCharacter.IsMatch(testName);
        }

        private const string PATH = @"Assets\_\Levels";
        private const string ASSET = "asset";
    }
}