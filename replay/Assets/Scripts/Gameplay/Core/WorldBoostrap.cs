namespace Gameplay.Core {
    public abstract class WorldBootstrap {
        protected readonly World _world;
        protected readonly WorldFlag _flag;
        
        protected WorldBootstrap(World world, WorldFlag flag)
        {
            _world = world;
            _flag = flag;
        }
        
        public abstract void Initialize();
    }
}