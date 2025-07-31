using UnityEngine;

namespace KillCam {
    public struct CharacterInputData {
        public Vector2Int Move;
        public float Yaw;
        public float Pitch;
        public bool HasValidInput() => Move != default;
    }
}