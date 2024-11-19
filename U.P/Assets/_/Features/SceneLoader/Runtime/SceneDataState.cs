using SceneLoader.Data;
using UnityEngine;

namespace SceneLoader.Runtime
{
    public class SceneDataState : MonoBehaviour
    {
        #region Private & Protected
        public static SceneDataState Instance { get; private set; }
        [SerializeField]
        private SceneData sceneData;
        #endregion

        #region Unity API

        private void Awake()
        {
            if (!Instance && FindObjectsOfType(GetType()).Length == 1)
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
