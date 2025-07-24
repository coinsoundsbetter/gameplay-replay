using FishNet.Managing;

namespace KillCam.Client
{
    public class ClientInitialize : InitializeFeature
    {
        private readonly Network network;
        private SpawnProvider spawnProvider;

        public ClientInitialize(NetworkManager manager)
        {
            network = new Network(manager);
        }

        public override void OnCreate()
        {
            ClientData.Create();
            ClientWorldsChannel.Create();
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
            ClientData.Destroy();
        }

        private void AddClientFeatures()
        {
            world.Add(network);
            world.Add(spawnProvider = new SpawnProvider());
            world.Add(new ActorManager());
            world.Add(new CharacterManager(spawnProvider));
            world.Add(new CameraManager());
        }
    }
}