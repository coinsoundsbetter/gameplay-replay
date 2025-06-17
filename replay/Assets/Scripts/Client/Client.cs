using Core;

namespace KillCam
{
    internal class Client : IGameLoop
    {
        private GameWorld gameWorld;

        internal void Init()
        {
            gameWorld = new GameWorld();
            var init = gameWorld.GetOrCreateSystemGroup<InitializeSystemGroup>();
            var simulate = gameWorld.GetOrCreateSystemGroup<SimulationSystemGroup>();
            var presentation = gameWorld.GetOrCreateSystemGroup<PresentationSystemGroup>();
        }

        internal void Clear()
        {
            gameWorld.Dispose();
        }

        public void OnUpdate()
        {
            gameWorld.OnUpdate();
        }

        public void OnLateUpdate()
        {
            gameWorld.OnLateUpdate();
        }

        public void OnFixedUpdate()
        {
            gameWorld.OnFixedUpdate();
        }
    }
}