using System.Runtime.CompilerServices;
using SceneLoader.Data;
using UnityEngine;

namespace SceneLoader.Runtime
{
    public class SceneDataLoader : MonoBehaviour
    {
        #region Private & Protected
        public static SceneDataLoader Instance { get; private set; }
        [SerializeField]
        private SceneData sceneData;
        #endregion

        #region Unity API

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(this);
            }
            
            if(sceneData == null)
            {
                throw new System.Exception($"No scene set loaded in scene loader.");
            }
            sceneData!.LoadScenes();
        }

        public void SetData(SceneData sceneData)
        {
            this.sceneData = sceneData;
        }
        #endregion
    }
}
