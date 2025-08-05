using FishNet.Managing;

namespace KillCam.Client {
    [UnityEngine.Scripting.Preserve]
    public class ClientBoostrap : ClientBoostrapBase {
        private readonly NetworkClient network;
        private SpawnProvider spawnProvider;

        public ClientBoostrap(NetworkManager manager) : base(manager) {
            network = new NetworkClient(manager);
        }

        protected override void OnBeforeInitialize() {
            ClientData.Create();
            ClientWorldsChannel.Create();
            AddClientFeatures();
            network.Start(() => { network.SendLoginRequest("Coin"); });
        }

        protected override void OnAfterCleanup() {
            network.Stop();
            ClientWorldsChannel.Destroy();
            ClientData.Destroy();
        }
        
        private void AddClientFeatures() {
            MyWorldActor.SetupData(new WorldTime());
            MyWorldActor.SetupCapability(network, TickGroup.InitializeLogic);
            MyWorldActor.SetupCapability(spawnProvider = new SpawnProvider(), TickGroup.InitializeLogic);
            MyWorldActor.SetupCapability<S2CHandle>(TickGroup.InitializeLogic);
            MyWorldActor.SetupCapability(new HeroManager(spawnProvider), TickGroup.InitializeLogic);
            MyWorldActor.SetupCapability<CameraManager>(TickGroup.CameraFrame);
        }
    }
}