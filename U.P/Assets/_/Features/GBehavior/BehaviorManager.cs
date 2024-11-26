using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBehavior
{
    public class BehaviorManager: MonoBehaviour
    {
        public static BehaviorManager Instance { get; private set; }
        private List<GBehavior> behaviors = new();

        private void Awake()
        {
            Instance ??= this;
            if (FindObjectsOfType<BehaviorManager>().Length >= 1)
            {
                DestroyImmediate(gameObject);
            }
        }

        public void Add(GBehavior behavior)
        {
            if (behaviors.Contains(behavior)) return;
            behaviors.Add(behavior);
        }

        public void Remove(GBehavior behavior)
        {
            if (!behaviors.Contains(behavior)) return;
            behaviors.Remove(behavior);
        }

        private void Update()
        {
            for (int i = 0; i < behaviors.Count; i++) behaviors[i].OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < behaviors.Count; i++) behaviors[i].OnFixedUpdate(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < behaviors.Count; i++) behaviors[i].OnLateUpdate(Time.deltaTime);
        }
    }
}