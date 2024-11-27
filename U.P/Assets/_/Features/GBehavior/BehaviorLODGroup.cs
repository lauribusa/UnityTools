using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBehavior
{
    /*
     * Thing to keep in mind with a LOD system;
     * it's not well adapted for games where the camera moves a lot and rapidly.
     * Objects may not have registered themselves to a higher LOD group by the time the camera is close
     * causing a "lag" in their behavior
     * e.g. an Object whose LODgroup only updates every 10 ticks might be at tick 3
     * by the time the camera is close,
     * and doesn't update fast enough to catch up when the camera reaches the Object.
     */
    [Serializable]
    public struct UpdateModel
    {
        public UpdateModel(int stride)
        {
            Stride = stride;
            TickCount = default;
            DeltaTime = default;
            UnscaledDeltaTime = default;
        }
        public readonly int Stride;
        public int TickCount;
        public float DeltaTime;
        public float UnscaledDeltaTime;

        public void Reset()
        {
            DeltaTime = 0;
            UnscaledDeltaTime = 0;
            TickCount = 0;
        }
    }

    [Serializable]
    public class UpdateLODGroup
    {
        public UpdateLODGroup(int stride = 1, int fixedStride = 1)
        {
            _update = new UpdateModel(stride);
            _fixedUpdate = new UpdateModel(fixedStride);
            _lateUpdate = new UpdateModel(stride);
        }
        private readonly List<GBehavior> _behaviors = new();
        
        [SerializeField]
        private UpdateModel _update;
        private UpdateModel _fixedUpdate;
        private UpdateModel _lateUpdate;

        public GBehavior Closest;
        public GBehavior Furthest;

        public void TickUpdate()
        {
            _update.TickCount++;
            _update.DeltaTime += Time.deltaTime;
            _update.UnscaledDeltaTime += Time.unscaledDeltaTime;
            if (_update.TickCount < _update.Stride) return;
            for (int i = 0; i < _behaviors.Count; i++)
            {
                _behaviors[i].OnUpdate(_update.DeltaTime,_update.UnscaledDeltaTime, _update.TickCount);
            }
            _update.Reset();
        }

        public void TickFixedUpdate()
        {
            _fixedUpdate.TickCount++;
            _fixedUpdate.DeltaTime += Time.fixedDeltaTime;
            _fixedUpdate.UnscaledDeltaTime += Time.fixedUnscaledDeltaTime;
            if (_fixedUpdate.TickCount < _fixedUpdate.Stride) return;
            for (int i = 0; i < _behaviors.Count; i++)
            {
                _behaviors[i].OnFixedUpdate(_fixedUpdate.DeltaTime,_fixedUpdate.UnscaledDeltaTime, _fixedUpdate.TickCount);
            }
            _fixedUpdate.Reset();
        }

        public void TickLateUpdate()
        {
            _lateUpdate.TickCount++;
            _lateUpdate.DeltaTime += Time.deltaTime;
            _lateUpdate.UnscaledDeltaTime += Time.unscaledDeltaTime;
            if (_lateUpdate.TickCount < _lateUpdate.Stride) return;
            for (int i = 0; i < _behaviors.Count; i++)
            {
                _behaviors[i].OnLateUpdate(_lateUpdate.DeltaTime,_lateUpdate.UnscaledDeltaTime, _lateUpdate.TickCount);
            }
            _lateUpdate.Reset();
        }

        public void Add(GBehavior behavior)
        {
            if (_behaviors.Contains(behavior)) return;
            _behaviors.Add(behavior);
        }

        public void Remove(GBehavior behavior)
        {
            if(!_behaviors.Contains(behavior)) return;
            _behaviors.Remove(behavior);
        }
    }
}