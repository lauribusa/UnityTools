using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBehavior
{
    public class BehaviorManager: MonoBehaviour
    {
        public static BehaviorManager Instance { get; private set; }
        private readonly List<UpdateLODGroup> _updateLodGroups = new();

        private const int STRIDE = 1000;
        private const int FIXED_STRIDE = 1000;

        private void Awake()
        {
            Instance ??= this;
            if (FindObjectsOfType<BehaviorManager>().Length > 1)
            {
                throw new UnityException("More than one BehaviorManager in the scene");
            }
            _updateLodGroups.Add(new UpdateLODGroup(STRIDE, FIXED_STRIDE));
        }

        public void Test()
        {
            
        }

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
            for (int i = 0; i < _updateLodGroups.Count; i++) _updateLodGroups[i].TickUpdate();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _updateLodGroups.Count; i++) _updateLodGroups[i].TickFixedUpdate();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _updateLodGroups.Count; i++) _updateLodGroups[i].TickLateUpdate();
        }
    }
}