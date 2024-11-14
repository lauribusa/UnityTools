using SceneLoader.Data;
using UnityEditor;
using OnOpenAsset = UnityEditor.Callbacks.OnOpenAssetAttribute;

namespace SceneLoader.Editor
{
    [CustomEditor(typeof(SceneData))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        private static SceneData _loadedData;
        [OnOpenAsset]
        public static bool OnDoubleClick(int instanceId, int line, int row)
        {
            var target = EditorUtility.InstanceIDToObject(instanceId);
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
            _loadedData = sceneData;
        }

        public static void UnloadSceneEditor()
        {
            if(_loadedData != null)
            {
                _loadedData.CloseSceneEditor();
            }
        }
    }
}
