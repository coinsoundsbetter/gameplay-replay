using Unity.Collections;
using Unity.Entities;

namespace KillCam {
    public struct WaitSpawnPlayer : IComponentData {
        public int PlayerId;
        public int NetId;
        public FixedString32Bytes PlayerName;
    }

    public struct PlayerTag : IComponentData {
        public int Id;
        public bool IsLocalPlayer;
        public FixedString32Bytes Name;
    }

    public class PlayerView : IComponentData
    {
        public IPlayerViewBinder Binder;
    }
}