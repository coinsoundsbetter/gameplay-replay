using Mix;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Client {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct ClientConnectSystem : ISystem {
        
        public void OnUpdate(ref SystemState state) {
            if (!SystemAPI.HasSingleton<NetworkId>()) {
                var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
                var conn = cmdBuffer.CreateEntity();
                var endPoint = NetworkEndpoint.LoopbackIpv4.WithPort(7979);
                cmdBuffer.AddComponent(conn, new NetworkStreamRequestConnect() {
                    Endpoint = endPoint,
                });
                cmdBuffer.Playback(state.EntityManager);
                Debug.Log("try connect to server...");
            }

            if (SystemAPI.HasSingleton<NetworkId>()) {
                var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
                var req = cmdBuffer.CreateEntity();
                cmdBuffer.AddComponent(req, new LoginRequest() {
                    PlayerName = "Coin",
                });
                cmdBuffer.AddComponent(req, new SendRpcCommandRequest() {
                    TargetConnection = Entity.Null,
                });
                cmdBuffer.Playback(state.EntityManager);
                Debug.Log("send login request to server...");
            }
        }
    }
}