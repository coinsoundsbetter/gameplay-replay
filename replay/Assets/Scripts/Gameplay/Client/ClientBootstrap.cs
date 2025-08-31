using FishNet.Managing;
using Gameplay.Core;

namespace Gameplay.Client {

    public class ClientBootstrapContext {
        public NetworkManager netPlugin;
    }
    
    [UnityEngine.Scripting.Preserve]
    public class ClientBootstrap : WorldBootstrap {
        public ClientBootstrap(World myWorld, WorldFlag flag) : base(myWorld, flag) { }

        public override void Initialize(NetworkManager manager) {
            myWorld.ActorManager.AddSingletonManaged(new ClientBootstrapContext() {
                netPlugin = manager,
            });
            
            var logicRoot = myWorld.LogicRoot;
            var initialize = new InitializeSystemGroup(myWorld);
            SystemCollector.CollectInto(initialize, WorldFlag.Client);
            logicRoot.AddSystem(initialize);
        }
    }
}