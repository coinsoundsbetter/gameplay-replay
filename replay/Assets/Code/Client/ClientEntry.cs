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
            world.SetupData(new UserInputData());
            
            // 网络相关
            world.SetNetworkContext(network);
            world.SetupFeature(network);
            world.SetupFeature(spawnProvider = new SpawnProvider());
            world.SetupFeature<NetMsgHandle>(TickGroup.NetworkReceive);
            
            // 输入
            world.SetupFeature<InputManager>(TickGroup.Input);
            
            // 逻辑模拟
            world.SetupFeature(new HeroManager(spawnProvider));
            world.SetupFeature<ProjectileManager>();
            
            // 表现相关
            world.SetupFeature<HudManager>(TickGroup.PostVisual);
            world.SetupFeature<CameraManager>(TickGroup.PostVisual);
        }
    }
}