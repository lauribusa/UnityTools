#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace _.Features.AddressableTools.Data
{
    
    [CreateAssetMenu(menuName = "Addressables/Addressable Definition", fileName = "AssetDefinition")]
    public class AddressableDefinition: ScriptableObject
    {
        public string AddressableName
        {
            get;
            set;
        }

        private void Awake()
        {
            #if UNITY_EDITOR
            if(string.IsNullOrWhiteSpace(AddressableName)) RenameSelfFromFolderName();
            #endif
        }
        
        private async void RenameSelfFromFolderName()
        {
            await Task.Delay(1);
            var path = AssetDatabase.GetAssetPath(GetInstanceID());
            var folderName = path.Split("/")[^1];
            var fileName = folderName.Split(".")[0];
            var errorInfo = AssetDatabase.RenameAsset(path, folderName);
            AddressableName = fileName;
            if(!string.IsNullOrWhiteSpace(errorInfo)) Debug.LogError(errorInfo);
        }
    }
}