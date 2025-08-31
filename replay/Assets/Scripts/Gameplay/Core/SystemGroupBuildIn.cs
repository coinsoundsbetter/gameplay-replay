namespace Gameplay.Core {
    
    public class InitializeSystemGroup : SystemGroup {
        public InitializeSystemGroup(World world) : base(world) {
        }
    }
    
    public class VisualizeSystemGroup : SystemGroup {
        public VisualizeSystemGroup(World world) : base(world) {
        }
    }
}