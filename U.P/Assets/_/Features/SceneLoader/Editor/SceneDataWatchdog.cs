using Paps.UnityToolbarExtenderUIToolkit;
using SceneLoader.Data;
using Toolbar.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesScan", ToolbarAlign.Right, order: 3)]
    public class SceneDataScanButton: Button, IToolbarElement
    {
        public void InitializeElement()
        { 
            text = "Scan";
            clicked += OnClick;
            style.display = DisplayStyle;
            ToolEvent.OnChanged += OnToolbarTypeChanged;
        }

        private static void OnClick()
        {
            SceneDataWatchdog.Scan();
        }

        [Serialize]
        public DisplayStyle DisplayStyle { get; set; }
        
        public string Type { get; set; } = ToolType.SCENE_DATA;
        public void OnToolbarTypeChanged(string value)
        {
            style.display = value == Type ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    
    public static class SceneDataWatchdog
    {
        private const string PATH = @"Assets\_\Levels";
        private const string ASSET = "asset";
        private const string SCENES_FOLDER = "scenes";
        public static void Scan()
        {
            var subDirectories = AssetDatabase.GetSubFolders(PATH);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            
            for (int i = 0; i < subDirectories.Length; i++)
            {
                var path = subDirectories[i];
                var sceneDataName = path.Split("/")[^1];
                var scenesPath = $"{path}/{SCENES_FOLDER}/";
                var assetPath = $"{path}/{sceneDataName}.{ASSET}";
                
                var group = settings.FindGroup(sceneDataName) ?? settings.CreateGroup(sceneDataName, false, false, true, null);
                var allScenes = AssetDatabase.FindAssets("t:scene", new[] { scenesPath });
                var sceneAssetReferences = SetScenesAsAddressable(allScenes, group);
                
                var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(assetPath);
                sceneData.sceneAssetReferences = sceneAssetReferences;
                AssetDatabase.SaveAssetIfDirty(sceneData);
            }
        }

        private static void FindAllAssetReferences(AddressableAssetGroup group)
        {
            group.GetAssetEntry("");
        }
        private static AssetReference[] SetScenesAsAddressable(string[] sceneGuids, AddressableAssetGroup group)
        {
            var sceneAssetReferences = new AssetReference[sceneGuids.Length];
            for (int i = 0; i < sceneGuids.Length; i++)
            {
                var sceneAssetReference = new AssetReference(sceneGuids[i]);
                var existingReference = group.GetAssetEntry(sceneGuids[i]);
                if (existingReference != null)
                {
                    sceneAssetReferences[i] = sceneAssetReference;
                    continue;
                }
                //TODO: skip if reference exists
;               var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                var addressableAssetEntry = SetAsAddressable(scenePath, group);
                addressableAssetEntry.labels.Add("scene");
                sceneAssetReferences[i] = sceneAssetReference;
            }
            return sceneAssetReferences;
        }
// debugwatch sur le git de cherif
// serializable callback de unity
        private static AddressableAssetEntry SetAsAddressable(string path, AddressableAssetGroup group, bool clearLabels = false)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var addressableAssetEntry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
            addressableAssetEntry.address = path;
            if(clearLabels) addressableAssetEntry.labels.Clear();
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, addressableAssetEntry, false);
            return addressableAssetEntry;
        }
    }
}