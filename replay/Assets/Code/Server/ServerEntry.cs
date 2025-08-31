using FishNet.Managing;

namespace KillCam.Server {
    [UnityEngine.Scripting.Preserve]
    public class ServerEntry : ServerEntryBase {
        private readonly NetworkServer networkServer;

        public ServerEntry(NetworkManager manager) : base(manager) {
            networkServer = new NetworkServer(manager);
        }
        
        protected override void OnBeforeStart() {
            var logicRoot = world.LogicRoot;
            var init = SystemCollector.Collect<InitializeSystemGroup>(WorldFlag.Server);
            var netRecv = SystemCollector.Collect<NetworkReceiveSystemGroup>(WorldFlag.Server);
            var input   = SystemCollector.Collect<InputSystemGroup>(WorldFlag.Server);
            var physics = SystemCollector.Collect<PhysicsSystemGroup>(WorldFlag.Server);
            var netSend = SystemCollector.Collect<NetworkSendSystemGroup>(WorldFlag.Server);
            logicRoot.AddSystem(init);
            logicRoot.AddSystem(netRecv);
            logicRoot.AddSystem(input);
            logicRoot.AddSystem(physics);
            logicRoot.AddSystem(netSend);
        }

        protected override void OnAfterDestroy() {
            networkServer.Stop();
        }
    }
}