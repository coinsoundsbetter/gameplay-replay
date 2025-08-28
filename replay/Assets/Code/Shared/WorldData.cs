using UnityEngine;

namespace KillCam {
    public struct NetworkTime {
        public uint Tick;
    }
    
    public struct NetworkData {
        public uint Tick;
        public long RTT;
        public long HalfRTT;
    }

    public struct UserInputState {
        public Vector2Int Move;
        public float Yaw;
        public float Pitch;
        public bool IsFirePressed;
    }
}