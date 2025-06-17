using Unity.Entities;
using Unity.NetCode;

namespace Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerGoInGameSytsem : ISystem 
    {
        public void OnUpdate(ref SystemState state)
        {
            var cmdBuffer = SystemAPI
                .GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .ValueRW.CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (netId, entity) in SystemAPI
                         .Query<RefRO<NetworkId>>()
                         .WithNone<NetworkStreamInGame>()
                         .WithEntityAccess())
            {
                cmdBuffer.AddComponent<NetworkStreamInGame>(entity);
            }
        }
    }
}