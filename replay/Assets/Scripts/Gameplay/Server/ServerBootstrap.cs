using FishNet.Managing;
using Gameplay.Core;

namespace Gameplay.Server {
    [UnityEngine.Scripting.Preserve]
    public class ServerBootstrap : WorldBootstrap {
        public ServerBootstrap(World myWorld, WorldFlag flag) : base(myWorld, flag) { }
        private NetworkServer server;
        
        public override void Initialize(NetworkManager manager) {
            NetMessageBoostrap.Initialize();
            server = new NetworkServer();
            server.Prepare(manager, myWorld.ActorManager);
            server.OnServerStarted = OnServerStarted;
            server.Start();
        }

        private void OnServerStarted() {
            // ==========
            // 逻辑系统组
            // ==========
            var logicRoot = myWorld.LogicRoot;
            var initialize = new InitializeSystemGroup(myWorld);
            initialize.AddSystem(new Server_LoginSystem());
            SystemCollector.CollectInto(initialize, SystemFlag.Server);
            logicRoot.AddSystem(initialize);
            
            var simulation = new SimulationSystemGroup(myWorld);
            SystemCollector.CollectInto(simulation, SystemFlag.Server);
            logicRoot.AddSystem(simulation);
        }

        public override void Dispose() {
            server.OnServerStarted = null;
            server.Stop();
        }
    }
}