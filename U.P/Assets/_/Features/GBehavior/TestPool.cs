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
        private float _delay = 1;
        private float _timer;
        private bool _isFired;

        public void Start()
        {
            SpawnNumber();
        }

        public void Update()
        {
            if (_timer < _delay)
            {
                _timer += Time.deltaTime;
                return;
            }
            if (_isFired) return;
            _isFired = true;
            PoolManager.PrintAllPools();
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