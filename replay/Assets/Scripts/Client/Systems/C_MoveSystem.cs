using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct C_MoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, entity) in SystemAPI
                         .Query<RefRO<PlayerTag>>()
                         .WithEntityAccess())
            {
                var binder = SystemAPI.ManagedAPI.GetComponent<PlayerView>(entity);
                var forward = binder.Binder.GetRotation() * Vector3.forward;
            }
        }
    }
}