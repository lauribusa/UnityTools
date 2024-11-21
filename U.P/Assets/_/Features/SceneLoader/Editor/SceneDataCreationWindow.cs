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
    /*
     * Créer un scriptableobject qui contiendra le nom des addressable group
pouvoir modifier la création de level
créer un script editor qui va les scanner
faire un diff (modifié par rapport au groupe, si oui je rebuilde)

- create scriptableobject "addressable definition"
- edit "new level logic" to add into scriptableobject
- "scan everything" to get all files that need to be changed into addressables
- make a diff with all groups (and perhaps change the UI to mark it red or green on add, remove, etc)
- Apply to all groups set as Dirty
- changer le sceneloader pour accepter des addressables
     */
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
            if (_isFocused) return;
            _isFocused = true;
            _sceneDataName.Focus();
        }

        private void CreateNewSceneData(string sceneDataName)
        {
            var sceneData = CreateInstance<SceneData>();
            var addressableDefinition = CreateInstance<AddressableDefinition>();
            sceneData.name = sceneDataName;
            AssetDatabase.CreateAsset(sceneData, $@"{PATH}\{sceneDataName}.{ASSET}");
            var sceneDataPath = $@"{PATH}\{sceneDataName}.{ASSET}";
            AssetDatabase.CreateAsset(addressableDefinition, $@"{PATH}\{sceneDataName}.{nameof(AddressableDefinition)}.{ASSET}");
            AssetDatabase.SaveAssetIfDirty(sceneData);
            AssetDatabase.SaveAssetIfDirty(addressableDefinition);
            ShowNotification(new GUIContent("Created Asset."));
            System.Threading.Tasks.Task.Delay(100);
            EditorGUIUtility.PingObject(sceneData);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.DefaultGroup;
            var sceneDataReference = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(sceneDataPath), group, false, false);
            sceneDataReference.address = sceneDataPath;
            sceneDataReference.labels.Clear();
            AssetDatabase.SetLabels(addressableDefinition, new []{"ignore"});
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, sceneDataReference, false, false);
            addressableDefinition.AddressableName = sceneDataName;
            Debug.Log($"{addressableDefinition.AddressableName}");
        }
        
        private static bool FileAlreadyExists(string fileName)
        {
            if (!File.Exists($@"{PATH}\{fileName}.{ASSET}")) return false;
            EditorUtility.DisplayDialog("Error", $@"File at path {PATH}\{fileName}.{ASSET} already exists.", "OK");
            return true;
        }

        private static bool IsValidFilename(string testName)
        {
            var containsABadCharacter = new Regex("["+ Regex.Escape(new string(Path.GetInvalidFileNameChars())) +"]");
            return !containsABadCharacter.IsMatch(testName);
        }

        private const string PATH = @"Assets\_\Features\SceneLoader\Data";
        private const string ASSET = "asset";
    }
}