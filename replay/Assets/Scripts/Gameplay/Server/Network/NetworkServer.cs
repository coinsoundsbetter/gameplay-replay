using System;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Transporting;
using Gameplay.Core;

namespace Gameplay.Server {
    
    public class NetworkServer {
        public NetworkManager UsePlugin { get; private set; }
        private ActorManager actorManager;
        private readonly Dictionary<int, NetSync> activeClients = new();
        public bool IsActive { get; set; }
        public Action OnServerStarted;

        public void Prepare(NetworkManager plugin, ActorManager manager) {
            actorManager = manager;
            UsePlugin = plugin;
            UsePlugin.ServerManager.OnServerConnectionState += OnServerConnState;
            NetSync.OnAddServer = OnAddServer;
            NetSync.OnRemoveServer = OnRemoveServer;
        }

        public void Start() {
            UsePlugin.ServerManager.StartConnection();
        }

        public void Stop() {
            UsePlugin.ServerManager.StopConnection(false);
            NetSync.OnAddServer = null;
            NetSync.OnRemoveServer = null;
            activeClients.Clear();
        }

        private void OnServerConnState(ServerConnectionStateArgs args) {
            switch (args.ConnectionState) {
                case LocalConnectionState.Stopping:
                    break;
                case LocalConnectionState.Started:
                    IsActive = true;
                    OnServerStarted?.Invoke();
                    break;
                default:
                    IsActive = false;
                    break;
            }
        }

        private void OnAddServer(NetSync obj) {
            activeClients.Add(obj.Id.Value, obj);
        }

        private void OnRemoveServer(NetSync obj) {
            activeClients.Remove(obj.Id.Value);
        }
        
        public void SendToClient<T>(int id, T message) where T : INetworkMessage {
            if (activeClients.TryGetValue(id, out var client)) {
                client.ServerRpc<T>(message);
            }
        }

        public void SendToAll<T>(T message) where T : INetworkMessage {
            foreach (var client in activeClients.Values) {
                client.ServerRpc<T>(message);
            }
        }
    }
}