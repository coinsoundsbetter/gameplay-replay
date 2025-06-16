using Mix;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Server {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ServerRpcReceiveSystem : ISystem {
        
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (rpc, entity) in SystemAPI
                         .Query<RefRO<CmdPlayerInput>>()
                         .WithEntityAccess()) {
                Debug.Log("Received CmdPlayerInput");
                ecb.DestroyEntity(entity);
            }
        }
    }
}