using Unity.Entities;

namespace KillCam {
    
    public partial struct C_LocalTickSystem : ISystem {

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetTickState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var tickState = SystemAPI.GetSingletonRW<NetTickState>();
            tickState.ValueRW.Local++;
        }
    }
}