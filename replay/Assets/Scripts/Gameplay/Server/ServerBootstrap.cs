using FishNet.Managing;
using Gameplay.Core;

namespace Gameplay.Server {
    
    [UnityEngine.Scripting.Preserve]
    public class ServerBootstrap : WorldBootstrap {
        public ServerBootstrap(World myWorld, WorldFlag flag) : base(myWorld, flag) { }
        
        public override void Initialize(NetworkManager manager) {
            
        }
    }
}