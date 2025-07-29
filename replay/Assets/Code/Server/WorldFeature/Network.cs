using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KillCam.Server {
    public class Network : Feature, INetwork {
        private NetworkManager manager;
        private Action startAction;
        private int clientUniqueId;

        public Network(NetworkManager networkManager) {
            manager = networkManager;
        }

        public void Start(Action started) {
            startAction = started;
            world.SetNetwork(this);
            manager.ServerManager.OnServerConnectionState += OnConnectState;
            manager.ServerManager.RegisterBroadcast<Login>(OnLogin);
            manager.ServerManager.StartConnection();
        }

        public void Stop() {
            manager.ServerManager.StopConnection(false);
            manager.ServerManager.OnServerConnectionState -= OnConnectState;
            manager.ServerManager.UnregisterBroadcast<Login>(OnLogin);
            world.RemoveNetwork(this);
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
            var asset = Resources.Load<GameObject>("RoleNet");
            var instance = Object.Instantiate(asset);
            var role = instance.GetComponent<RoleNet>();
            role.Id.Value = ++clientUniqueId;
            var networkObj = instance.GetComponent<NetworkObject>();
            manager.ServerManager.Spawn(networkObj, conn);
        }

        public void Send(INetworkMsg data) {
        }

        public void Rpc(INetworkMsg data) {
            var roleMgr = Get<CharacterManager>();
            foreach (var actor in roleMgr.RoleActors.Values) {
                actor.Net?.Rpc(data);
            }
        }

        public new uint GetTick() {
            return manager.TimeManager.LocalTick;
        }

        public void Send<T>(T message) where T : INetworkMsg { }
        public void Rpc<T>(T message) where T : INetworkMsg { }
        public void TargetRpc<T>(int id, T message) where T : INetworkMsg { }
    }
}