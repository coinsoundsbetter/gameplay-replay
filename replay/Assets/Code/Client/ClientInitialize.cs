using FishNet.Managing;

namespace KillCam.Client
{
    public class ClientInitialize : InitializeFeature
    {
        private readonly Client_Network network;

        public ClientInitialize(NetworkManager manager)
        {
            network = new Client_Network(manager);
        }

        public override void OnCreate()
        {
            ClientWorldsChannel.Create();
            world.Add(network);
            AddClientFeatures();
            network.Start(() =>
            {
                network.SendLoginRequest("Coin");
            });
        }

        public override void OnDestroy()
        {
            network.Stop();
            ClientWorldsChannel.Destroy();
        }

        private void AddClientFeatures()
        {
            world.Add(new Client_RoleManager());
        }
    }
}