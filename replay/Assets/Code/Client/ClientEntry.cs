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
            world.SetupData(new NetworkTime());
            world.SetupData(new NetworkData());
            world.SetupData(new CameraData());
            world.SetupBuffer<ImpactData>();
            
            // 网络相关
            world.SetNetworkContext(network);
            world.CreateFeature(network);
            world.CreateFeature(spawnProvider = new SpawnProvider());
            world.CreateFeature<NetMsgHandle>(TickGroup.NetworkReceive);
            
            // 输入
            world.CreateFeature<GatherInputSystem>(TickGroup.Input);
            
            // 逻辑模拟
            world.CreateFeature(new HeroSpawnSystem(spawnProvider));
            world.CreateFeature<Projectiles>(TickGroup.Simulation);
            
            // 表现相关
            world.CreateFeature<ProjectileImpacts>(TickGroup.Visual);
            world.CreateFeature<VisualHUD>(TickGroup.PostVisual);
            world.CreateFeature<CameraManager>(TickGroup.PostVisual);
            
            world.SetupAllFeatures();
        }
    }
}