using FishNet.Managing;
using FishNet.Transporting;

namespace KillCam.Server
{
    public class ServerInitialize : InitializeFeature
    {
        private NetworkManager _manager;
        
        public ServerInitialize(NetworkManager manager)
        {
            _manager = manager;
        }

        public override void OnCreate()
        {
            _manager.ServerManager.OnServerConnectionState += OnServerState;
            _manager.ServerManager.StartConnection();
        }

        public override void OnDestroy()
        {
            _manager.ServerManager.StopConnection(true);
            _manager.ServerManager.OnServerConnectionState -= OnServerState;
        }

        private void OnServerState(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started)
            {
                AddFeatures(world);
            }
        }
        
        private void AddFeatures(BattleWorld bw)
        {
            bw.Add(new BaseFeature_ServerLogin(_manager));
            bw.Add(new BaseFeature_ServerSpawn(_manager));
            bw.Add(new BaseFeature_C2SHandle());
        }
    }
}