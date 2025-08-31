using FishNet.Broadcast;
using FishNet.Managing;
using FishNet.Transporting;
using Gameplay.Core;
using Unity.Collections;

namespace Gameplay.Client {
    
    [Order(SystemOrder.First)]
    [WorldFilter(All = WorldFlag.Client)]
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public class Client_ConnectSystem : SystemBase {
        private NetworkManager plugin;
        
        protected override void OnCreate() {
            plugin = Actors.GetSingletonManaged<ClientBootstrapContext>().netPlugin;
            Actors.CreateSingleton(new ConnectionInfo() {
                State = ConnectState.Offline,
            });
            plugin.ClientManager.OnClientConnectionState += OnConnectionState;
        }

        protected override void OnDestroy() {
            plugin.ClientManager.OnClientConnectionState -= OnConnectionState;
        }

        protected override void OnStartRunning() {
            plugin.ClientManager.StartConnection();
        }

        protected override void OnStopRunning() {
            plugin.ClientManager.StopConnection();
        }

        protected override void OnUpdate() {
            // 发送登录请求
            ref var info = ref Actors.GetSingleton<ConnectionInfo>();
            if (!info.HasLogin && info.State == ConnectState.Connected) {
                info.HasLogin = true;
                plugin.ClientManager.Broadcast(new LoginRequest() {
                    PlayerName = "Coin",
                });
            }
        }

        private void OnConnectionState(ClientConnectionStateArgs args) {
            ref var info = ref Actors.GetSingleton<ConnectionInfo>();
            switch (args.ConnectionState) {
                case LocalConnectionState.Started:
                    info.State = ConnectState.Connected;
                    break;
                case LocalConnectionState.Starting:
                    info.State = ConnectState.Connecting;
                    break;
                case LocalConnectionState.Stopping:
                case LocalConnectionState.Stopped:
                    info.State = ConnectState.Offline;
                    break;
            }
        }
    }
    
    public enum ConnectState {
        Offline,
        Connecting,
        Connected,
    }
    
    public struct ConnectionInfo {
        public ConnectState State;
        public bool HasLogin;
    }

    public struct LoginRequest : IActorData, IBroadcast {
        public FixedString32Bytes PlayerName;
    }
}