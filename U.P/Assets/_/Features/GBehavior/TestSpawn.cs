using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GBehavior
{
    public class AddressableTesting : GBehavior
    {
        [SerializeField] private AssetReference _assetReference;
        [SerializeField] private Pool _pool;

        internal override async void Awake()
        {
            Debug.Log($"<color=cyan>[Test] 1: spawning 1</color>");
            Spawn<Object>(_assetReference, 5, OnSpawnPoolCompleted); // 1/5

            Debug.Log($"<color=cyan>[Test] 2: spawning 3</color>");
            for (int i = 0; i < 3; i++)
            {
                Spawn<Object>(_assetReference, 4, OnSpawnPoolCompleted); // 4/5
            }

            await Task.Delay(2000);
            Debug.Log($"<color=cyan>[Test] 3: spawning 1</color>");
            Spawn<Object>(_assetReference, 3,OnSpawnPoolCompleted); // 5/5

            await Task.Delay(2000);
            Debug.Log($"<color=cyan>[Test] 4: spawning 1</color>");
            Spawn<Object>(_assetReference, 8, OnSpawnPoolCompleted); // 6/8

            await Task.Delay(2000);
            Debug.Log($"<color=cyan>[Test] 5: spawning 4</color>");
            Spawn<Object>(_assetReference, 8, OnSpawnPoolCompleted);  // 7/8
            Spawn<Object>(_assetReference, 8, OnSpawnPoolCompleted);  // 8/8
            Spawn<Object>(_assetReference, 8, OnSpawnPoolCompleted);  // 9/9!!!
            Spawn<Object>(_assetReference, 8, OnSpawnPoolCompleted);  // 10/10!!!
            Spawn<Object>(_assetReference, 9, OnSpawnPoolCompleted);  // 11/11!!!
        }

        private void OnSpawnCompleted(Object go)
        {
            Debug.Log($"<color=cyan>[Test] {go.name} is loaded</color>");
            _gameObject = (GameObject)go;
        }

        private void OnSpawnPoolCompleted(Pool pool)
        {
            Debug.Log($"<color=cyan>[Test] 1: {pool.Size} is loaded</color>");
            _pool = pool;
        }

        private GameObject _gameObject;
    } 
}
