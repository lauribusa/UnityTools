using Unity.Entities;
using UnityEngine;

namespace GBehavior.UpdateLOD
{
    public struct UpdateLODDistanceData: IComponentData
    {
        public float SquaredDistance;
        public Vector3 Position;
        public Transform Transform;
    }
}