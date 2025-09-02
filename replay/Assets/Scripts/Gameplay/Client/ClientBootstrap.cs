using FishNet.Managing;
using Gameplay.Core;
using Unity.Collections;

namespace Gameplay.Client {

    public struct UserInGame : IActorData {
        public FixedString32Bytes UserName;
    }
    
    [UnityEngine.Scripting.Preserve]
    public class ClientBootstrap : WorldBootstrap {
        private NetworkClient client;
        
        public ClientBootstrap(World myWorld, WorldFlag flag) : base(myWorld, flag) { }

        public override void Initialize(NetworkManager manager) {
            NetMessageBootstrap.Initialize();
            client = myWorld.ActorManager.CreateSingletonManaged<NetworkClient>();
            client.Prepare(manager, myWorld.ActorManager);
            TestInGame.LoginEvent += OnTestInGame;
        }
        
        public override void Dispose() {
            client.StopConnection();
        }
        
        private void OnTestInGame(string userName) {
            StartGame();
        }

        private void StartGame() {
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
            
            client.StartConnection();
        }
    }
}