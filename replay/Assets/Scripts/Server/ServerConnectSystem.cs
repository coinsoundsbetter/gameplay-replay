using Mix;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Server {
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ServerConnectSystem : ISystem {
        
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<LoginRequest>();    
        }

        public void OnUpdate(ref SystemState state) {
            var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (req, entity) in SystemAPI
                         .Query<RefRO<LoginRequest>>()
                         .WithEntityAccess()) {
                Debug.Log("receive login request");
                cmdBuffer.DestroyEntity(entity);
            }
        }
    }
}