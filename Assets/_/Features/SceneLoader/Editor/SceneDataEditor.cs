using SceneLoader.Data;
using UnityEditor;
using UnityEngine;
using OnOpenAsset = UnityEditor.Callbacks.OnOpenAssetAttribute;

namespace SceneLoader.Editor
{
    [CustomEditor(typeof(SceneData))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        static SceneData loadedData;
        [OnOpenAsset]
        public static bool OnDoubleClick(int instanceId, int line, int row)
        {
            Object target = EditorUtility.InstanceIDToObject(instanceId);

            switch (target)
            {
                case SceneData sceneData:
                    if(loadedData != null)
                    {
                        loadedData.CloseSceneEditor();
                    }
                    sceneData.LoadScenesEditor();
                    loadedData = sceneData;                    
                    return true;
                default:
                    return false;
            }
        }
    }
}
