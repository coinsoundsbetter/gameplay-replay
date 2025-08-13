using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KillCam.Server {
    public class NetworkServer : Feature, INetworkContext {
        private readonly NetworkManager manager;
        private Action startAction;
        private int clientUniqueId;
        public bool IsServer { get; } = true;
        public bool IsClient { get; } = false;

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
            ref var worldTime = ref GetWorldDataRW<WorldTime>();
            worldTime.Tick = manager.TimeManager.LocalTick;
            ref var networkData = ref GetWorldDataRW<NetworkData>();
            networkData.RTT = manager.TimeManager.RoundTripTime;
            networkData.HalfRTT = manager.TimeManager.HalfRoundTripTime;
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
        
        public void SendToServer<T>(T message) where T : INetworkMsg {
            throw new InvalidOperationException("Server doesn't send to server");
        }

        public void SendToAllClients<T>(T message) where T : INetworkMsg {
            var roleMgr = GetWorldFeature<HeroManager>();
            foreach (var actor in roleMgr.RoleActors.Values) {
                actor.GetDataManaged<ServerHeroNetLink>()?.NetServer?.Rpc(message);
            }
        }

        public void SendToClient<T>(int playerId, T message) where T : INetworkMsg {
            var roleMgr = GetWorldFeature<HeroManager>();
            if (roleMgr.RoleActors.TryGetValue(playerId, out var role)) {
                role.GetDataManaged<ServerHeroNetLink>()?.NetServer.Rpc(message);
            }
        }

        public new uint GetTick() {
            var worldTime = GetWorldDataRO<WorldTime>();
            return worldTime.Tick;
        }

        public new double GetNowTime() {
            return manager.TimeManager.TicksToTime(GetTick());
        }

        public new long GetRTT() {
            var networkData = GetWorldDataRO<NetworkData>();
            return networkData.RTT;
        }

        public new long GetHalfRTT() {
            var networkData = GetWorldDataRO<NetworkData>();
            return networkData.HalfRTT;
        }
    }
}