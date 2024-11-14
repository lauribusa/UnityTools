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
                    LoadSceneEditor(sceneData);
                    return true;
                default:
                    return false;
            }
        }

        public static void LoadSceneEditor(SceneData sceneData)
        {
            UnloadSceneEditor();
            sceneData.LoadScenesEditor();
            loadedData = sceneData;
        }

        public static void UnloadSceneEditor()
        {
            if(loadedData != null)
            {
                loadedData.CloseSceneEditor();
            }
        }
    }
}
