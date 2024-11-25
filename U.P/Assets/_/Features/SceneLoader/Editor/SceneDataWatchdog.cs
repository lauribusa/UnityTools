using System.Collections.Generic;
using Paps.UnityToolbarExtenderUIToolkit;
using SceneLoader.Data;
using Toolbar.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace SceneLoader.Editor
{
    [MainToolbarElement("ScenesScan", ToolbarAlign.Right, order: 3)]
    public class SceneDataScanButton : Button, IToolbarElement
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

        [Serialize] public DisplayStyle DisplayStyle { get; set; }

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

                var group = settings.FindGroup(sceneDataName) ??
                            settings.CreateGroup(sceneDataName, false, false, true, null);

                //TODO: don't just scan for scenes but instead look for AddressableDefinition and set ALL items within folder as addressable, while ignoring those set as "ignore".
                var addressableDefinition = AssetDatabase.FindAssets("t:AddressableDefinition", new[] { path });
                if (addressableDefinition.Length <= 0) continue;

                var scenesGuids = AssetDatabase.FindAssets("t:scene", new[] { scenesPath });
                var assetsGuids = AssetDatabase.FindAssets(null, new[] {path});
                var all = new string[scenesGuids.Length + assetsGuids.Length];
                
                scenesGuids.CopyTo(all, 0);
                assetsGuids.CopyTo(all, scenesGuids.Length);
                
                var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(assetPath);
                var assetReferences = SetAssetsAsAddressable(all, group, sceneData);
                sceneData.sceneAssetReferences = assetReferences;
                AssetDatabase.SaveAssetIfDirty(sceneData);
            }
        }

        private static bool MustBeIgnored(string[] labels)
        {
            for (var index = 0; index < labels.Length; index++)
            {
                if (labels[index] is "ignore" or "Ignore") return true;
            }

            return false;
        }

        private static AssetReference[] SetAssetsAsAddressable(string[] guids, AddressableAssetGroup group, SceneData sceneData)
        {
            var assetReferences = new AssetReference[guids.Length];
            var scenes = new List<AssetReference>();
            for (int i = 0; i < guids.Length; i++)
            {
                var labels = AssetDatabase.GetLabels(new GUID(guids[i]));
                var ignored = MustBeIgnored(labels);
                if (ignored) continue;
                var assetReference = new AssetReference(guids[i]);
                var existingReference = group.GetAssetEntry(guids[i]);
                if (existingReference != null)
                {
                    assetReferences[i] = assetReference;
                    continue;
                }

                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var addressableAssetEntry = SetAsAddressable(assetPath, group);
                if (addressableAssetEntry.IsScene)
                {
                    addressableAssetEntry.labels.Add("scene");
                    scenes.Add(assetReference);
                }
                assetReferences[i] = assetReference;
                Debug.Log($"{addressableAssetEntry.address}");
            }
            
            sceneData.sceneAssetReferences = scenes.ToArray();
            return assetReferences;
        }

// debugwatch sur le git de cherif
// serializable callback de unity
        private static AddressableAssetEntry SetAsAddressable(string path, AddressableAssetGroup group,
            bool clearLabels = false)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var addressableAssetEntry =
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
            addressableAssetEntry.address = path;
            if (clearLabels) addressableAssetEntry.labels.Clear();
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, addressableAssetEntry, false);
            return addressableAssetEntry;
        }
    }
}