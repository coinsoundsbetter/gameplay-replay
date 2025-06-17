/*using Mix;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

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
                // 这里加一些不知道用来干啥的东西,看起来NetCode必须要它们
                cmdBuffer.AddComponent<NetworkStreamInGame>(entity);
                cmdBuffer.AddComponent<ConnectionState>(entity);

                // 发送登录请求
                var req = cmdBuffer.CreateEntity();
                cmdBuffer.AddComponent(req, new CmdPlayerLogin()
                {
                    PlayerName = "Coin"
                });
                cmdBuffer.AddComponent(req, new SendRpcCommandRequest()
                {
                    TargetConnection = entity,
                });
                Debug.Log("发送登录请求...");
            }

            cmdBuffer.Playback(state.EntityManager);
            cmdBuffer.Dispose();
        }
    }
}*/