using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Server {

    public struct HeroSpawner : IBufferElement {
        public int Id;
        public bool IsReLogin;
    }
    
    /// <summary>
    /// 服务器登录校验
    /// </summary>
    [DisableCollector]
    public class Server_LoginSystem : SystemBase {
        private NetworkServer server;
        private Dictionary<string, int> loginPlayers = new Dictionary<string, int>();
        private int nextId;

        protected override void OnCreate() {
            Actors.CreateSingletonBuffer<HeroSpawner>();
            server = Actors.GetSingletonManaged<NetworkServer>();
            server.UsePlugin.ServerManager.RegisterBroadcast<LoginRequest>(OnLogin);
        }

        protected override void OnDestroy() {
            server = Actors.GetSingletonManaged<NetworkServer>();
            server.UsePlugin.ServerManager.UnregisterBroadcast<LoginRequest>(OnLogin);
        }

        private void OnLogin(NetworkConnection conn, LoginRequest request, Channel channel) {
            var connection = Resources.Load<GameObject>("NetConnection");
            var connectionObj = Object.Instantiate(connection);
            var networkObj = connectionObj.GetComponent<NetworkObject>();
            var netSync = connectionObj.GetComponent<NetSync>();
            netSync.Id.Value = ++nextId;
            connectionObj.name = $"Net_{netSync.Id.Value}";
            Actors.GetSingletonManaged<NetworkServer>().UsePlugin.ServerManager.Spawn(networkObj, conn);
        }
    }
}