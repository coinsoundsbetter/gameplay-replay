using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Mix {
    public partial struct ServerPlayerSpawnSystem : ISystem {
        
        public void OnUpdate(ref SystemState state) {
            var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (conn, entity) in SystemAPI
                         .Query<RefRO<NetworkStreamConnection>>()
                         .WithNone<PlayerSpawner>()
                         .WithEntityAccess()) {
                
                
                
            }
        }
    }
}