using Gameplay.Core;

namespace Gameplay.Server {
    
    [UnityEngine.Scripting.Preserve]
    public class ServerBootstrap : WorldBootstrap {
        public ServerBootstrap(World world, WorldFlag flag) : base(world, flag) {
        }

        public override void Initialize() {
            
        }
    }
}