using FishNet.Managing;

namespace KillCam.Client {
    [UnityEngine.Scripting.Preserve]
    public class ClientEntry : ClientEntryBase {
        private readonly NetworkClient network;
        private SpawnProvider spawnProvider;

        public ClientEntry(NetworkManager manager) : base(manager) {
            network = new NetworkClient(manager);
        }

        protected override void OnBeforeStart() {
            AppData.Create();
            ClientWorldsChannel.Create();
            AddClientFeatures();
            network.Start(() => { network.SendLoginRequest("Coin"); });
        }

        protected override void OnAfterDestroy() {
            network.Stop();
            ClientWorldsChannel.Destroy();
            AppData.Destroy();
        }
        
        private void AddClientFeatures() {
            world.SetupData(new WorldTime());
            world.SetupFeature(network, TickGroup.InitializeLogic);
            world.SetupFeature(spawnProvider = new SpawnProvider(), TickGroup.InitializeLogic);
            world.SetupFeature<NetMsgHandle>(TickGroup.InitializeLogic);
            world.SetupFeature(new HeroManager(spawnProvider), TickGroup.InitializeLogic);
            world.SetupFeature<HudManager>(TickGroup.InitializeFrame);
            world.SetupFeature<CameraManager>(TickGroup.CameraFrame);
        }
    }
}