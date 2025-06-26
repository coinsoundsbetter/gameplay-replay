using Unity.Entities;

namespace KillCam {
    public partial struct C_DebugSystem : ISystem {
        public void OnUpdate(ref SystemState state) {

            if (SystemAPI.HasSingleton<NetTickState>()) {
                var tickState = SystemAPI.GetSingleton<NetTickState>();
                ClientLog.Log($"本地:{tickState.Local} - 远端:{tickState.Remote}");
            }
        }
    }
}