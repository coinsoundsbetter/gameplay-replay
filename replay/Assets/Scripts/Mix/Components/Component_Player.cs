using Unity.Collections;
using Unity.Entities;

namespace KillCam {
    public struct WaitSpawnPlayer : IComponentData {
        public int PlayerId;
        public FixedString32Bytes PlayerName;
    }

    public struct PlayerTag : IComponentData {
        public int Id;
        public FixedString32Bytes Name;
    }

    public class PlayerView : IComponentData
    {
        public IPlayerViewBinder Binder;
    }
}