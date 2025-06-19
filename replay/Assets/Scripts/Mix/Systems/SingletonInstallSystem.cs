using Unity.Entities;

namespace KillCam {
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial struct SingletonInstallSystem : ISystem {
        
        public void OnCreate(ref SystemState state) {
            var entity = state.EntityManager.CreateSingleton<SingletonTag>();
            state.EntityManager.AddComponent<GameIdPool>(entity);
            state.EntityManager.AddComponentObject(entity, new GameDict());
            state.Enabled = false;
        }
    }
}