using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using Unity.Entities;

namespace KillCam {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(InitializationSystemGroup))]
    public partial class S_ConnectSystem : SystemBase {
        private NetworkManager manager;
        private int playerIdIndex;

        protected override void OnCreate() {
            manager = InstanceFinder.NetworkManager;
            manager.ServerManager.OnRemoteConnectionState += OnRemoteClientState;
            manager.ServerManager.StartConnection();
        }

        protected override void OnDestroy() {
            manager.ServerManager.StopConnection(false);
            manager.ServerManager.OnRemoteConnectionState -= OnRemoteClientState;
        }

        private void OnRemoteClientState(NetworkConnection conn, RemoteConnectionStateArgs args) {
            if (args.ConnectionState == RemoteConnectionState.Started) {
                var clientEntity = EntityManager.CreateEntity(ComponentType.ReadWrite<NetConnection>());
                EntityManager.SetComponentData(clientEntity, new NetConnection() {
                    NetId = args.ConnectionId,
                    PlayerId = ++playerIdIndex,
                    NetState = NetConnectState.Connected,
                });

                var reqCreatePlayer = EntityManager.CreateEntity(ComponentType.ReadWrite<WaitSpawnPlayer>());
                EntityManager.SetComponentData(reqCreatePlayer, new WaitSpawnPlayer() {
                    PlayerId = playerIdIndex,
                    PlayerName = "Player_" + playerIdIndex,
                });
            }
        }

        protected override void OnUpdate() {
            
        }
    }
}