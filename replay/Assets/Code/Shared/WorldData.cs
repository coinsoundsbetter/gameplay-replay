using UnityEngine;

namespace KillCam {
    public struct WorldTime {
        public uint Tick;
    }

    public struct InputData {
        public Vector2Int Move;
        public float Yaw;
        public float Pitch;
        public bool IsFirePressed;
    }
}