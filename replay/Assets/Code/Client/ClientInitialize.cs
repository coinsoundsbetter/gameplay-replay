using FishNet.Managing;
using FishNet.Transporting;

namespace KillCam.Client
{
    public class ClientInitialize : InitializeFeature
    {
        private NetworkManager _manager;

        public ClientInitialize(NetworkManager manager)
        {
            _manager = manager;
        }

        public override void OnCreate()
        {
            AddClientFeatures();
            _manager.ClientManager.OnClientConnectionState += OnLocalConnectState;
            _manager.ClientManager.StartConnection();
        }

        public override void OnDestroy()
        {
            _manager.ClientManager.StopConnection();
            _manager.ClientManager.OnClientConnectionState -= OnLocalConnectState;
        }

        private void OnLocalConnectState(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started)
            {
                _manager.ClientManager.Broadcast<Login>(new Login()
                {
                    UserName = "Coin",
                });    
            }
        }

        private void AddClientFeatures()
        {
            world.Add(new Client_BaseFeature_Spawn(_manager));
            world.Add(new Client_BaseFeature_StateDriver());
        }
    }
}