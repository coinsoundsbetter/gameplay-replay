using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Transporting;
using Gameplay.Core;

namespace Gameplay.Client {
    public struct ConnectionInfo {
        public ConnectState State;
    }
    
    public enum ConnectState {
        Offline,
        Connecting,
        Connected,
    }
    
    public class NetworkClient {
        public NetworkManager usePlugin { get; private set; }
        private ActorManager actorManager;
        public bool IsActive { get; set; }
        private readonly Dictionary<int, NetSync> localClients = new ();

        public void Prepare(NetworkManager plugin, ActorManager manager) {
            usePlugin = plugin;
            actorManager = manager;
            actorManager.CreateSingleton(new ConnectionInfo() {
                State = ConnectState.Offline,
            });
            actorManager.CreateSingletonBuffer<AddClientEvent>();
            actorManager.CreateSingletonBuffer<RemoveClientEvent>();
            IsActive = false;
        }
        
        public void StartConnection() {
            NetSync.OnAddClient = OnAddClient;
            NetSync.OnRemoveClient = OnRemoveClient;
            usePlugin.ClientManager.OnClientConnectionState += OnClientConnState;
            usePlugin.ClientManager.StartConnection();
        }

        public void StopConnection() {
            usePlugin.ClientManager.StopConnection();
            NetSync.OnAddClient = null;
            NetSync.OnRemoveClient = null;
            usePlugin.ClientManager.OnClientConnectionState -= OnClientConnState;
        }
        
        private void OnClientConnState(ClientConnectionStateArgs args) {
            if (!actorManager.HasSingleton<ConnectionInfo>()) {
                return;
            }
            
            ref var data = ref actorManager.GetSingleton<ConnectionInfo>();
            switch (args.ConnectionState) {
                case LocalConnectionState.Started:
                    data.State = ConnectState.Connected;
                    break;
                case LocalConnectionState.Starting:
                    data.State = ConnectState.Connecting;
                    break;
                case LocalConnectionState.Stopped:
                    data.State = ConnectState.Offline;
                    break;
            }
        }
        
        private void OnAddClient(NetSync obj) {
            localClients.Add(obj.Id.Value, obj);
            ref var events = ref actorManager.GetSingletonBuffer<AddClientEvent>();
            events.Add(new AddClientEvent() {
                Id = obj.Id.Value,
                IsLocalPlayer = obj.IsOwner,
            });
        }
        
        private void OnRemoveClient(NetSync obj) {
            localClients.Remove(obj.Id.Value);
            ref var events = ref actorManager.GetSingletonBuffer<RemoveClientEvent>();
            events.Add(new RemoveClientEvent() {
                Id = obj.Id.Value,
            });
        }
    }
}