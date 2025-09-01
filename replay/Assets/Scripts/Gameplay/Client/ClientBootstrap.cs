using FishNet.Managing;
using Gameplay.Core;

namespace Gameplay.Client {
    
    [UnityEngine.Scripting.Preserve]
    public class ClientBootstrap : WorldBootstrap {
        private NetworkClient client;
        
        public ClientBootstrap(World myWorld, WorldFlag flag) : base(myWorld, flag) { }

        public override void Initialize(NetworkManager manager) {
            NetMessageBoostrap.Initialize();
            
            client = new NetworkClient();
            client.Prepare(manager, myWorld.ActorManager);
            
            // ==========
            // 逻辑系统组
            // ==========
            var logicRoot = myWorld.LogicRoot;
            var initialize = new InitializeSystemGroup(myWorld);
            SystemCollector.CollectInto(initialize, SystemFlag.Client);
            logicRoot.AddSystem(initialize);
            
            var simulation = new SimulationSystemGroup(myWorld);
            SystemCollector.CollectInto(simulation, SystemFlag.Client);
            logicRoot.AddSystem(simulation);
            
            // ==========
            // 渲染系统组
            // ==========
            var frameRoot = myWorld.FrameRoot;
            var visualization = new VisualizeSystemGroup(myWorld);
            SystemCollector.CollectInto(visualization, SystemFlag.Client);
            frameRoot.AddSystem(visualization);
            
            client.Start();
        }

        public override void Dispose() {
            client.Stop();
        }
    }
}