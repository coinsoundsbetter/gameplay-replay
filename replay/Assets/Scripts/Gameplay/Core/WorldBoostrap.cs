using FishNet.Managing;

namespace Gameplay.Core {
    public abstract class WorldBootstrap {
        protected readonly World myWorld;
        protected readonly WorldFlag _flag;
        
        protected WorldBootstrap(World world, WorldFlag flag)
        {
            myWorld = world;
            _flag = flag;
        }
        
        public abstract void Initialize(NetworkManager manager);
        public abstract void Dispose();
    }
}