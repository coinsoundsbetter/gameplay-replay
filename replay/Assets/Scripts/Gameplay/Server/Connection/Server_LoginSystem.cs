using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Transporting;
using Gameplay.Core;

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
            if (!loginPlayers.ContainsKey(request.PlayerName)) {
                var id = ++nextId;
                loginPlayers.Add(request.PlayerName, id);
                ref var buffer = ref Actors.GetSingletonBuffer<HeroSpawner>();
                buffer.Add(new HeroSpawner() {
                    Id = id,
                    IsReLogin = false,
                });
            }
        }
    }
}