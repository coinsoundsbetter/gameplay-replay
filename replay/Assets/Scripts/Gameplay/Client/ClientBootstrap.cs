using Gameplay.Core;

namespace Gameplay.Client {
    
    [UnityEngine.Scripting.Preserve]
    public class ClientBootstrap : WorldBootstrap {
        public ClientBootstrap(World world, WorldFlag flag) : base(world, flag) {
        }

        public override void Initialize() {
            var logicRoot = _world.LogicRoot;
            var initialize = new InitializeSystemGroup(_world);
            SystemCollector.CollectInto(initialize, WorldFlag.Client);
            logicRoot.AddSystem(initialize);
        }
    }
}