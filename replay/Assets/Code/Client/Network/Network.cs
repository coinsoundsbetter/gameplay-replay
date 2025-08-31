using System;
using FishNet.Managing;
using FishNet.Transporting;

namespace KillCam.Client {
    public class NetworkClient : INetworkContext {
        private readonly NetworkManager manager;
        private IClientHeroNet sender;
        private Action startAction;
        public event Action<IClientHeroNet> AfterRoleSpawn;
        public event Action<IClientHeroNet> AfterRoleDespawn;
        
        public bool IsServer { get; } = false;
        public bool IsClient { get; } = true;

        public NetworkClient(NetworkManager mgr) {
            manager = mgr;
        }
        
        /*protected override void OnCreate() {
            CreateData<NetworkTime>();
        }*/

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

        /*protected override void OnTick() {
            ref var worldTime = ref GetWorldDataRef<NetworkTime>();
            worldTime.Tick = manager.TimeManager.LocalTick;
            ref var networkData = ref GetWorldDataRef<NetworkData>();
            networkData.RTT = manager.TimeManager.RoundTripTime;
            networkData.HalfRTT = manager.TimeManager.HalfRoundTripTime;
        }*/
        
        public void SendToServer<T>(T message) where T : INetworkMsg {
            sender?.Send(message);
        }

        public new void SendToAllClients<T>(T message) where T : INetworkMsg {
            throw new InvalidOperationException("Client can't broadcast to all");
        }

        public new void SendToClient<T>(int playerId, T message) where T : INetworkMsg {
            throw new InvalidOperationException("Client can't send to specific client");
        }

        public new uint GetTick() {
            /*var worldData = GetWorldData<NetworkTime>();
            return worldData.Tick;*/
            return 0;
        }
        
        public new double GetNowTime() {
            return manager.TimeManager.TicksToTime(GetTick());
        }

        public new long GetRTT() {
            /*var networkData = GetWorldData<NetworkData>();
            return networkData.RTT;*/
            return 0;
        }

        public new long GetHalfRTT() {
            /*var networkData = GetWorldData<NetworkData>();
            return networkData.HalfRTT;*/
            return 0;
        }
    }
}