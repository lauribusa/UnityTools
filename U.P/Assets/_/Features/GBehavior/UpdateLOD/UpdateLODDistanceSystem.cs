using Unity.Entities;
using UnityEngine;

namespace GBehavior.UpdateLOD
{
    public partial struct UpdateLODDistanceSystem: ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            
        }
    }

    public partial struct UpdateLODDistanceJob : IJobEntity
    {
        public void Execute(ref UpdateLODDistanceData distance, in Transform origin, in Transform target)
        {
            
        }
    }
}