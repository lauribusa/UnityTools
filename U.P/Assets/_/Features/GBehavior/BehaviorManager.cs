using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

namespace GBehavior
{
    public class BehaviorManager: MonoBehaviour
    {
        // private List<UpdateLODGroup> _updateLodGroups;
        public static BehaviorManager Instance { get; private set; }
        private BehaviorLODGroup[] _updateLodGroups;

        private const int STRIDE = 1000;
        private const int FIXED_STRIDE = 1000;
        
        private Stopwatch _stopwatch;
        
        public AssetReference reference;
        
        public void Add(GBehavior behavior)
        {
           _updateLodGroups[0].Add(behavior);
        }

        public void Remove(GBehavior behavior)
        {
            _updateLodGroups[0].Remove(behavior);
        }

        private void Update()
        {
            for (int i = 0; i < _updateLodGroups.Length; i++) _updateLodGroups[i].TickUpdate();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _updateLodGroups.Length; i++) _updateLodGroups[i].TickFixedUpdate();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _updateLodGroups.Length; i++) _updateLodGroups[i].TickLateUpdate();
        }
        
        private void Start()
        {
            MemoryStressTest();
        }
        
        private void Awake()
        {
            Instance ??= this;
            if (FindObjectsOfType<BehaviorManager>().Length > 1)
            {
                throw new UnityException("More than one BehaviorManager in the scene");
            }

            var newUpdateGroup = new BehaviorLODGroup(STRIDE, FIXED_STRIDE);
            _updateLodGroups = new[] { newUpdateGroup };
        }

        private void MemoryStressTest()
        {
            const int count = 10000;
            _stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                GBehavior.Spawn(reference, OnComplete);
            }
        }

        private int _count = 0;
        private void OnComplete(GameObject obj)
        {
            _count++;
            if (_count < 10000) return;
            _stopwatch.Stop();
            var elapsed = _stopwatch.ElapsedMilliseconds;
            Debug.Log($"Time elapsed for 10000 objects: {elapsed} ms.");
        }
    }
}