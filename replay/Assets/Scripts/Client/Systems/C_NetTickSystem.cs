using FishNet;
using FishNet.Managing;
using Unity.Entities;

namespace KillCam {
    /// <summary>
    /// 客户端知道自己的滴答跟当前已进展到的远端滴答
    /// </summary>
    public partial class C_NetTickSystem : SystemBase {
        private NetworkManager manager;

        protected override void OnCreate()
        {
            manager = InstanceFinder.NetworkManager;
        }

        protected override void OnUpdate()
        {
            var netTick = SystemAPI.GetSingletonRW<NetTickState>();
            netTick.ValueRW.Local = manager.TimeManager.LocalTick;
            netTick.ValueRW.Remote = manager.TimeManager.Tick;
        }
    }
}