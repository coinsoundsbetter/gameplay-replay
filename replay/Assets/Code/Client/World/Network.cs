using System;
using FishNet.Managing;
using FishNet.Transporting;

namespace KillCam.Client {
    public class Network : Feature, INetwork {
        private readonly NetworkManager manager;
        private IClientRoleNet sender;
        private Action startAction;
        public event Action<IClientRoleNet> AfterRoleSpawn;
        public event Action<IClientRoleNet> AfterRoleDespawn;

        public Network(NetworkManager mgr) {
            manager = mgr;
        }

        public void Start(Action started) {
            startAction = started;
            world.SetNetwork(this);
            RoleNet.OnClientSpawn += OnClientSpawn;
            RoleNet.OnClientDespawn += OnClientDespawn;
            manager.ClientManager.OnClientConnectionState += OnLocalConnectState;
            manager.ClientManager.StartConnection();
        }

        public void Stop() {
            world.RemoveNetwork(this);
            manager.ClientManager.StopConnection();
            RoleNet.OnClientSpawn -= OnClientSpawn;
            RoleNet.OnClientDespawn -= OnClientDespawn;
            manager.ClientManager.OnClientConnectionState -= OnLocalConnectState;
        }

        private void OnClientSpawn(IClientRoleNet net) {
            if (net.IsClientOwned()) {
                sender = net;
            }

            AfterRoleSpawn?.Invoke(net);
        }

        private void OnClientDespawn(IClientRoleNet net) {
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

        public void Rpc(INetworkMsg data) {
        }

        public new uint GetTick() {
            return manager.TimeManager.LocalTick;
        }

        public void Send<T>(T message) where T : INetworkMsg {
            sender?.Send(message);
        }
        
        public void Rpc<T>(T message) where T : INetworkMsg { }
        
        public void TargetRpc<T>(int id, T message) where T : INetworkMsg { }
    }
}