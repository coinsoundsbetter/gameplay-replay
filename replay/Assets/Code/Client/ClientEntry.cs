using FishNet.Managing;
using UnityEngine;

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
            AddClientSystems();
            network.Start(() => { network.SendLoginRequest("Coin"); });
        }

        protected override void OnAfterDestroy() {
            network.Stop();
            ClientWorldsChannel.Destroy();
            AppData.Destroy();
        }

        private void AddClientSystems() {
            /*var logicRoot = world.LogicRoot;
            
            var init = new InitializeSystemGroup();
            init.AddSystem(new WorldTimeSystem());
            SystemCollector.CollectInto(init, WorldFlag.Client);
            logicRoot.AddSystem(init);

            var netRecv = new NetworkReceiveSystemGroup();
            SystemCollector.CollectInto(netRecv, WorldFlag.Client);
            logicRoot.AddSystem(netRecv);
            
            var input = new InputSystemGroup();
            SystemCollector.CollectInto(input, WorldFlag.Client);
            logicRoot.AddSystem(input);
            
            var physics = new PhysicsSystemGroup();
            SystemCollector.CollectInto(physics, WorldFlag.Client);
            logicRoot.AddSystem(physics);
            
            var netSend = new NetworkSendSystemGroup();
            SystemCollector.CollectInto(netSend, WorldFlag.Client);
            logicRoot.AddSystem(netSend);
            
            var frameRoot = world.FrameRoot;
            var visualize = new VisualizeSystemGroup();
            SystemCollector.CollectInto(visualize, WorldFlag.Client);
            frameRoot.AddSystem(visualize);

            var postVisualize = new PostVisualizeSystemGroup();
            SystemCollector.CollectInto(postVisualize, WorldFlag.Client);
            frameRoot.AddSystem(postVisualize);*/
        }
    }
}