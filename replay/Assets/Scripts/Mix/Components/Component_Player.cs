using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    public struct WaitSpawnPlayer : IComponentData {
        public int PlayerId;
        public int NetId;
        public FixedString32Bytes PlayerName;
    }

    public struct PlayerIdentifier : IComponentData {
        public int Id;
        public bool IsLocalPlayer;
        public FixedString32Bytes Name;
    }

    public struct PlayerMovementState : IComponentData
    {
        public Vector3 Pos;
        public Quaternion Rot;
    }

    public class PlayerView : IComponentData
    {
        public IPlayerViewBinder Binder;
    }
}