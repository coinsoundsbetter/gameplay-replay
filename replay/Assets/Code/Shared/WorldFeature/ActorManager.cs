using System.Collections.Generic;

namespace KillCam
{
    public class ActorManager : Feature
    {
        private readonly List<GameplayActor> _actors = new List<GameplayActor>();
        
        public override void OnCreate()
        {
            world.AddLogicUpdate(OnLogicUpdate);
            world.AddFrameUpdate(OnFrameUpdate);
        }

        public override void OnDestroy()
        {
            world.RemoveLogicUpdate(OnLogicUpdate);
            world.RemoveFrameUpdate(OnFrameUpdate);
        }

        private void OnLogicUpdate(double delta)
        {
            foreach (var actor in _actors)
            {
                actor.TickLogic(delta);
            }
        }
        
        private void OnFrameUpdate(float delta)
        {
            foreach (var actor in _actors)
            {
                actor.TickFrame(delta);
            }
        }
        
        public GameplayActor CreateActor()
        {
            var newActor = new GameplayActor();
            newActor.SetupWorld(world);
            _actors.Add(newActor);
            return newActor;
        }

        public void RegisterActor(GameplayActor actor)
        {
            actor.SetupWorld(world);
            _actors.Add(actor);
        }

        public void UnregisterActor(GameplayActor actor)
        {
            _actors.Remove(actor);
        }

        public void DestroyActor(GameplayActor actor)
        {
            _actors.Remove(actor);
        }
    }
}