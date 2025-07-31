using UnityEngine;

namespace KillCam {
    public struct CharacterInputData {
        public Vector2Int Move;
        public float MouseX;

        public bool HasValidInput() => Move != default ||
                                       MouseX != 0;
    }
}