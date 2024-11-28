using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

namespace Glue
{
    public partial struct BehaviorLODSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new BehaviorLODJob().Schedule();
        }

        public void OnCreate(ref SystemState state)
        {
            
        }
    }

    public struct BehaviorLODData : IComponentData
    {
        public float SquaredDistance;
        public TransformAccessArray CameraTransform;
        public int GameObjectID;
    }

    public partial struct BehaviorLODJob : IJobEntity
    {
        public void Execute(in LocalTransform self, ref BehaviorLODData data)
        {
            data.SquaredDistance = math.distancesq(data.CameraTransform[0].position, self.Position);
            Debug.Log($"{data.SquaredDistance} :: {data.GameObjectID}");
        }
    }
}
