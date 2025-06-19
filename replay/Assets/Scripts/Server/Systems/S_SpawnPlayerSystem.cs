using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct S_SpawnPlayerSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.EntityManager.CreateSingleton(new GameIdPool());
            state.RequireForUpdate<WaitSpawnPlayer>();
        }

        public void OnUpdate(ref SystemState state) {
            var cmd = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (spawn, ent) in SystemAPI
                         .Query<RefRO<WaitSpawnPlayer>>()
                         .WithEntityAccess()) {
                // 生成信息
                var playerId = spawn.ValueRO.PlayerId;
                var playerName = spawn.ValueRO.PlayerName;
                
                // 获取对应的角色网络Id
                int spawnPlayerNetId = 0;
                foreach (var conn in SystemAPI.Query<RefRO<NetConnection>>()) {
                    if (conn.ValueRO.PlayerId == playerId) {
                        spawnPlayerNetId = conn.ValueRO.NetId;
                        break;
                    }
                }
                
                if (spawnPlayerNetId == 0) {
                    Debug.LogError("无法找到角色网络Id");
                    continue;
                }
                
                // 命令销毁
                cmd.DestroyEntity(ent);
                
                // 生成服务器角色
                var entity = cmd.CreateEntity();
                cmd.AddComponent(entity, new PlayerTag() {
                    Id = playerId,
                    Name = playerName,
                });
                cmd.AddComponent(entity, new PlayerHealth());
                cmd.AddComponent(entity, new PlayerMovement() {
                    Position = Vector3.zero,
                    Rotation = Quaternion.identity,
                });
                
                // 通知客户端生成角色
                var rpc = cmd.CreateEntity();
                cmd.AddComponent(rpc, new S2C_NetSpawnPlayer() {
                    PlayerId = playerId,
                    PlayerName = playerName,
                    Pos = Vector3.zero,
                    Rot = Quaternion.identity,
                });
                cmd.AddComponent(rpc, new S2C_RpcAll());
            }
        }
    }
}