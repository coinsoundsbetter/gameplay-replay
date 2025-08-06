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
            world.AddWorldData(new WorldTime());
            world.AddWorldFunc(network, TickGroup.InitializeLogic);
            world.AddWorldFunc(spawnProvider = new SpawnProvider(), TickGroup.InitializeLogic);
            world.AddWorldFunc<NetMsgHandle>(TickGroup.InitializeLogic);
            world.AddWorldFunc(new HeroManager(spawnProvider), TickGroup.InitializeLogic);
            world.AddWorldFunc<CameraManager>(TickGroup.CameraFrame);
        }
    }
}