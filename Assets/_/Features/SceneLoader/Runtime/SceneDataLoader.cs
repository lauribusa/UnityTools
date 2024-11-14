using SceneLoader.Data;
using UnityEngine;

namespace SceneLoader.Runtime
{
    public class SceneDataLoader : MonoBehaviour
    {
        #region Private & Protected
        [SerializeField]
        private SceneData _sceneData;
        #endregion

        #region Unity API

        private void Awake()
        {
            if(_sceneData == null)
            {
                throw new System.Exception($"No scene set loaded in scene loader.");
            }
            _sceneData!.LoadScenes();
        }
        #endregion
    }
}
