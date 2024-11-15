using SceneLoader.Data;
using UnityEngine;

namespace SceneLoader.Runtime
{
    public class SceneDataLoader : MonoBehaviour
    {
        #region Private & Protected

        [SerializeField]
        private SceneData sceneData;
        #endregion

        #region Unity API

        private void Awake()
        {
            if(sceneData == null)
            {
                throw new System.Exception($"No scene set loaded in scene loader.");
            }
            sceneData!.LoadScenes();
        }
        #endregion
    }
}
