using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using Unity.Entities;

namespace KillCam {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateAfter(typeof(InitializationSystemGroup))]
    public partial class C_ConnectSystem : SystemBase {
        private NetworkManager manager;
        
        protected override void OnCreate() {
            EntityManager.CreateSingleton<ConnectState>();
            manager = InstanceFinder.NetworkManager;
            manager.ClientManager.OnClientConnectionState += OnConnectState;
            manager.ClientManager.StartConnection();
        }

        protected override void OnDestroy() {
            manager.ClientManager.OnClientConnectionState -= OnConnectState;
            manager.ClientManager.StopConnection();
        }

        protected override void OnUpdate() {
        }
        
        private void OnConnectState(ClientConnectionStateArgs args) {
            
        }
    }
}