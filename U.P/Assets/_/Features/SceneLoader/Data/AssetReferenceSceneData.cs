using System;
using UnityEngine.AddressableAssets;

namespace SceneLoader.Data
{
    [Serializable]
    public class AssetReferenceSceneData : AssetReferenceT<SceneData>
    {
        public AssetReferenceSceneData(string guid) : base(guid)
        {
        }
    }
}