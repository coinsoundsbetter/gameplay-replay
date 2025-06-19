using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    public struct WaitSpawnPlayer : IComponentData {
        public int PlayerId;
        public FixedString32Bytes PlayerName;
    }

    public struct PlayerTag : IComponentData {
        public int Id;
        public FixedString32Bytes Name;
    }

    public struct PlayerHealth : IComponentData {
        public int Hp;
        public int MaxHp;
    }

    public struct PlayerMovement : IComponentData {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
    }
}