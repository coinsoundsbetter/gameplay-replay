using FishNet;
using FishNet.Managing;
using Unity.Entities;

namespace KillCam {
    
    public partial struct C_NetTickSystem : ISystem {

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetTickState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            
        }

        /*protected override void OnCreate()
        {
            manager = InstanceFinder.NetworkManager;
        }

        protected override void OnUpdate()
        {
            var netTick = SystemAPI.GetSingletonRW<NetTickState>();
            netTick.ValueRW.Local = manager.TimeManager.LocalTick;
            netTick.ValueRW.Remote = manager.TimeManager.Tick;
        }*/
    }
}