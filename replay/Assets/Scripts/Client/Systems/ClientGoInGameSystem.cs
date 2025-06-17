using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ClientGoInGameSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_, entity) in SystemAPI
                         .Query<RefRO<NetworkId>>()
                         .WithNone<NetworkStreamInGame>()
                         .WithEntityAccess())
            {
                cmdBuffer.AddComponent<NetworkStreamInGame>(entity);
                cmdBuffer.AddComponent<ConnectionState>(entity);
            }

            cmdBuffer.Playback(state.EntityManager);
        }
    }
}