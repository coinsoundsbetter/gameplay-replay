namespace Gameplay.Core {
    public struct SystemState {
        public World World;
        public ActorManager ActorManager => World.ActorManager;
        public float DeltaTime;
    }
}