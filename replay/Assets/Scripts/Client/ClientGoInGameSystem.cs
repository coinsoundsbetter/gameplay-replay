using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Client {
    
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct ClientGoInGameSystem : ISystem {
        
        public void OnCreate(ref SystemState state) {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NetworkId>()
                .WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state) {
            var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (id, entity) in SystemAPI
                         .Query<RefRO<NetworkId>>()
                         .WithEntityAccess()
                         .WithNone<NetworkStreamInGame>()) {
                cmdBuffer.AddComponent<NetworkStreamInGame>(entity);
                var req = cmdBuffer.CreateEntity();
                cmdBuffer.AddComponent<GoInGameRequest>(req);
                cmdBuffer.AddComponent(req, new SendRpcCommandRequest());
            }
            
            cmdBuffer.Playback(state.EntityManager);
        }
    }
}

public struct GoInGameRequest : IRpcCommand { }