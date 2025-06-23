using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class S_LoginSystem : SystemBase
    {
        private NetworkManager manager;
        
        protected override void OnCreate()
        {
            manager = InstanceFinder.NetworkManager;
            manager.ServerManager.RegisterBroadcast<LoginRequest>(OnLoginRequest);
        }

        protected override void OnDestroy()
        {
            manager.ServerManager.UnregisterBroadcast<LoginRequest>(OnLoginRequest);
        }

        private void OnLoginRequest(NetworkConnection conn, LoginRequest req, Channel channel)
        {
            var data = SystemAPI.ManagedAPI.GetSingleton<GameData>();
            // 首次登录
            if (!data.UserNameToPlayerId.TryGetValue(req.UserName, out var pId))
            {
                int newId = ++data.GameIdPool;
                Debug.Log($"生成角色=>{req.UserName}, Id:{newId}");
                data.UserNameToPlayerId.Add(req.UserName, newId);
                var asset = Resources.Load("NetChannel");
                var instance = (GameObject)Object.Instantiate(asset);
                var netChannel = instance.GetComponent<FishNetChannel>();
                netChannel.PlayerId.Value = newId;
                manager.ServerManager.Spawn(instance, conn);
            }
            // 重登
            else
            {
                
            }
            
            // 通知所有客户端,这里有一个新的角色生成了
            SystemAPI.ManagedAPI.GetSingleton<RpcQueue>().Add(new S2C_NetSpawnPlayer()
            {
                PlayerId = pId,
                PlayerName = req.UserName,
                Pos = Vector3.zero,
                Rot = Quaternion.identity,
            });
        }

        protected override void OnUpdate()
        {
            
        }
    }
}