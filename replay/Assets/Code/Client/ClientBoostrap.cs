using FishNet.Managing;

namespace KillCam.Client {
    [UnityEngine.Scripting.Preserve]
    public class ClientBoostrap : ClientInitialize {
        private readonly Network network;
        private SpawnProvider spawnProvider;

        public ClientBoostrap(NetworkManager manager) : base(manager) {
            network = new Network(manager);
        }

        public override void OnCreate() {
            ClientData.Create();
            ClientWorldsChannel.Create();
            AddClientFeatures();
            network.Start(() => { network.SendLoginRequest("Coin"); });
        }

        public override void OnDestroy() {
            network.Stop();
            ClientWorldsChannel.Destroy();
            ClientData.Destroy();
        }

        private void AddClientFeatures() {
            world.Add(network);
            world.Add(spawnProvider = new SpawnProvider());
            world.Add(new S2CHandle());
            world.Add(new ActorManager());
            world.Add(new HeroManager(spawnProvider));
            world.Add(new CameraManager());
        }
    }
}