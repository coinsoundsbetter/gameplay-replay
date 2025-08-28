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
            world.AddData(new NetworkTime());
            world.AddData(new NetworkData());
            world.AddData(new CameraData());
            world.SetupBuffer<ImpactData>();
            
            // 网络相关
            world.SetNetworkContext(network);
            world.AddFeature(network);
            world.AddFeature(spawnProvider = new SpawnProvider());
            world.AddFeature<NetMsgHandle>(TickGroup.NetworkReceive);
            
            // 输入
            world.AddFeature<GatherInputSystem>(TickGroup.Input);
            
            // 逻辑模拟
            world.AddFeature(new Client_SpawnHeroSystem(spawnProvider));
            world.AddFeature<BulletMoveSystem>();
            
            // 表现相关
            world.AddFeature<VisualHUD>(TickGroup.PostVisual);
            world.AddFeature<CameraManager>(TickGroup.PostVisual);
            
            world.SetupAllFeatures();
        }
    }
}