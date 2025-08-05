using System;
using FishNet.Managing;
using FishNet.Transporting;

namespace KillCam.Client {
    public class NetworkClient : Capability, INetworkClient {
        private readonly NetworkManager manager;
        private IClientHeroNet sender;
        private Action startAction;
        public event Action<IClientHeroNet> AfterRoleSpawn;
        public event Action<IClientHeroNet> AfterRoleDespawn;

        public NetworkClient(NetworkManager mgr) {
            manager = mgr;
        }

        public void Start(Action started) {
            startAction = started;
            HeroNet.OnClientSpawn += OnClientSpawn;
            HeroNet.OnClientDespawn += OnClientDespawn;
            manager.ClientManager.OnClientConnectionState += OnLocalConnectState;
            manager.ClientManager.StartConnection();
        }

        public void Stop() {
            manager.ClientManager.StopConnection();
            HeroNet.OnClientSpawn -= OnClientSpawn;
            HeroNet.OnClientDespawn -= OnClientDespawn;
            manager.ClientManager.OnClientConnectionState -= OnLocalConnectState;
        }

        private void OnClientSpawn(IClientHeroNet net) {
            if (net.IsClientOwned()) {
                sender = net;
            }

            AfterRoleSpawn?.Invoke(net);
        }

        private void OnClientDespawn(IClientHeroNet net) {
            if (net.IsClientOwned()) {
                sender = null;
            }

            AfterRoleDespawn?.Invoke(net);
        }

        private void OnLocalConnectState(ClientConnectionStateArgs args) {
            if (args.ConnectionState == LocalConnectionState.Started) {
                startAction?.Invoke();
            }
        }

        public void SendLoginRequest(string userName) {
            manager.ClientManager.Broadcast<Login>(new Login() {
                UserName = "Coin",
            });
        }

        protected override void OnTickActive() {
            ref var worldTime = ref World.GetWorldDataRW<WorldTime>();
            worldTime.Tick = manager.TimeManager.LocalTick;
        }

        public void Send<T>(T message) where T : INetworkMsg {
            sender?.Send(message);
        }

        public uint GetTick() {
            var worldTime = World.GetWorldDataRO<WorldTime>();
            return worldTime.Tick;
        }

        public void Rpc<T>(T message) where T : INetworkMsg { }
        
        public void TargetRpc<T>(int id, T message) where T : INetworkMsg { }
    }
}