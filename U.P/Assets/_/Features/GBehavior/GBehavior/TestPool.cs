using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GBehavior
{
    public class TestPool: GBehavior
    {
        public List<Object> myObjects = new();
        public AssetReference prefab;
        public int count;

        public void Start()
        {
            SpawnNumber();
            base.Start();
        }

        private void SpawnNumber()
        {
            Spawn<GBehavior>(prefab,OnComplete, count);
        }

        private void OnComplete(Object obj)
        {
            var go = obj;
            myObjects.Add(go);
        }
    }
}