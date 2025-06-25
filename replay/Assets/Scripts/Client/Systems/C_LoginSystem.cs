using FishNet;
using FishNet.Broadcast;
using FishNet.Managing;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    public partial class C_LoginSystem : SystemBase
    {
        private NetworkManager manager;
        
        protected override void OnCreate()
        {
            manager = InstanceFinder.NetworkManager;
        }

        protected override void OnUpdate()
        {
            var cmd = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (connState, entity) in SystemAPI
                         .Query<RefRO<Event_ConnectState>>()
                         .WithEntityAccess())
            {
                if (connState.ValueRO is { Last: NetConnectState.Undefined, Now: NetConnectState.Connected })
                {
                    cmd.DestroyEntity(entity);
                    SendLoginRequest("Coin", "123456");
                    Debug.Log("请求登录!");
                }
            }
            cmd.Playback(EntityManager);
            cmd.Dispose();
        }

        private void SendLoginRequest(string userName, string token)
        {
            manager.ClientManager.Broadcast(new LoginRequest()
            {
                UserName = userName,
                Token = token,
            });
            
        }
    }

    public struct LoginRequest : IBroadcast
    {
        public string UserName;
        public string Token;
    }
}