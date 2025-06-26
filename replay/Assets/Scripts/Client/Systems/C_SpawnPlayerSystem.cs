using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    public partial struct C_SpawnPlayerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var cmd = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (data, entity) in SystemAPI
                         .Query<RefRO<S2C_NetSpawnPlayer>>()
                         .WithEntityAccess())
            {
                ClientLog.Log($"新角色加入世界! {data.ValueRO.PlayerName}");
                cmd.DestroyEntity(entity);
                SpawnPlayer(ref cmd, data.ValueRO);
            }
            cmd.Playback(state.EntityManager);
            cmd.Dispose();
        }

        private void SpawnPlayer(ref EntityCommandBuffer cmd, S2C_NetSpawnPlayer context)
        {
            var localPlayerId = SystemAPI.ManagedAPI.GetSingleton<NetChannels>().LocalPlayerId;
            var asset = Resources.Load<GameObject>("Player");
            var gameObj = Object.Instantiate(asset);
            var script = gameObj.GetComponent<IPlayerViewBinder>();
            var view = cmd.CreateEntity();
            cmd.AddComponent(view, new PlayerIdentifier()
            {
                Id = context.PlayerId,
                Name = context.PlayerName,
                IsLocalPlayer = context.PlayerId == localPlayerId,
            });
            cmd.AddComponent(view, new PlayerView()
            {
                Binder = script,
            });
            cmd.AddComponent(view, new PlayerMovementState()
            {
                Pos = context.Pos,
                Rot = context.Rot,
            });
            script.SetPos(context.Pos);
            script.SetRotation(context.Rot);
        }
    }
}