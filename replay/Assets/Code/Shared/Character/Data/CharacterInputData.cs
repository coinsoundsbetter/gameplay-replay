using UnityEngine;

namespace KillCam {
    public struct CharacterInputData {
        public Vector2Int Move;

        public bool HasValidInput() => Move != default(Vector2Int);
    }
}