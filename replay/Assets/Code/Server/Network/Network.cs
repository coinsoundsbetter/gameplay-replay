using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KillCam.Server {
    public class NetworkServer : Capability {
        private NetworkManager manager;
        private Action startAction;
        private int clientUniqueId;

        public NetworkServer(NetworkManager networkManager) {
            manager = networkManager;
        }

        public void Start(Action started) {
            startAction = started;
            manager.ServerManager.OnServerConnectionState += OnConnectState;
            manager.ServerManager.RegisterBroadcast<Login>(OnLogin);
            manager.ServerManager.StartConnection();
        }

        public void Stop() {
            manager.ServerManager.StopConnection(false);
            manager.ServerManager.OnServerConnectionState -= OnConnectState;
            manager.ServerManager.UnregisterBroadcast<Login>(OnLogin);
        }

        protected override void OnTickActive() {
            ref var worldTime = ref World.GetWorldDataRW<WorldTime>();
            worldTime.Tick = manager.TimeManager.LocalTick;
        }

        private void OnConnectState(ServerConnectionStateArgs args) {
            if (args.ConnectionState == LocalConnectionState.Started) {
                startAction?.Invoke();
            }
        }

        private void OnLogin(NetworkConnection conn, Login loginInfo, Channel channel) {
            SpawnRoleNet(conn, loginInfo);
        }

        private void SpawnRoleNet(NetworkConnection conn, Login loginInfo) {
            var asset = Resources.Load<GameObject>("Shared/CharacterNet");
            var instance = Object.Instantiate(asset);
            var role = instance.GetComponent<HeroNet>();
            role.Id.Value = ++clientUniqueId;
            var networkObj = instance.GetComponent<NetworkObject>();
            manager.ServerManager.Spawn(networkObj, conn);
        }

        public uint GetTick() {
            var worldTime = World.GetWorldDataRO<WorldTime>();
            return worldTime.Tick;
        }
        
        public void Rpc<T>(T message) where T : INetworkMsg {
            var roleMgr = World.GetFunction<HeroManager>();
            foreach (var actor in roleMgr.RoleActors.Values) {
                actor.GetDataManaged<ServerHeroNetLink>()?.NetServer?.Rpc(message);
            }
        }
        public void TargetRpc<T>(int id, T message) where T : INetworkMsg { }
    }
}