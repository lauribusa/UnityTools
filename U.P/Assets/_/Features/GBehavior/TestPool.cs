using System.Collections.Generic;
using System.Linq;
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

        internal override void Start()
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
            Spawn<GBehavior>(prefab,count,OnCompletePool);
        }

        private void OnCompletePool(Pool obj)
        {
            var go = obj;
        }
    }
}