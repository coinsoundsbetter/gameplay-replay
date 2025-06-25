using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    public partial struct S_SpawnPlayerSystem : ISystem {
        public void OnCreate(ref SystemState state) {
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
                var playerNetId = spawn.ValueRO.NetId;
                
                // 命令销毁
                cmd.DestroyEntity(ent);
                
                // 生成服务器角色
                var entity = cmd.CreateEntity();
                cmd.AddComponent(entity, new PlayerTag() {
                    Id = playerId,
                    Name = playerName,
                });
                
                // 通知客户端生成角色
                SystemAPI.ManagedAPI.GetSingleton<NetRpc>().Add(new S2C_NetSpawnPlayer()
                {
                    PlayerId = playerId,
                    PlayerName = playerName,
                    Pos = Vector3.zero,
                    Rot = Quaternion.identity,
                });
            }
            
            cmd.Playback(state.EntityManager);
            cmd.Dispose();
        }
    }
}