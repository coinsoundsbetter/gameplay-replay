using UnityEngine;

namespace KillCam {
    public struct WorldTime {
        public uint Tick;
    }

    public struct NetworkData {
        public long RTT;
        public long HalfRTT;
    }

    public struct UserInputData {
        public Vector2Int Move;
        public float Yaw;
        public float Pitch;
        public bool IsFirePressed;
    }
}