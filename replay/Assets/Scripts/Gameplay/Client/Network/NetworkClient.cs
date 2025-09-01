using System.Collections.Generic;
using FishNet.Managing;
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
            NetSync.OnAddClient = OnAddClient;
            NetSync.OnRemoveClient = OnRemoveClient;
            RegisterMessages();
        }

        private void RegisterMessages() {
            
        }

        public void Start() {
            usePlugin.ClientManager.StartConnection();
        }

        public void Stop() {
            usePlugin.ClientManager.StopConnection();
            NetSync.OnAddClient = null;
            NetSync.OnRemoveClient = null;
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