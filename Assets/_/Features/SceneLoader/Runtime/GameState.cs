using SceneLoader.Data;
using UnityEngine;

namespace SceneLoader.Runtime
{
    public class GameState: MonoBehaviour
    {
        public static GameState Instance;
        [SerializeField]
        private SceneData sceneData;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            DestroyImmediate(this);
        }

        public void LoadScene(SceneData scene)
        {
            if (sceneData != null)
            {
                UnloadScene(scene);
            }
            scene.LoadScenesRuntime();
        }

        public void UnloadScene(SceneData scene)
        {
            scene.CloseSceneRuntime();
        }
    }
}