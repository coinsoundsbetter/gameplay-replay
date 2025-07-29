using System;
using Unity.Collections;

namespace KillCam {
    public struct AllCharacterSnapshot : IDisposable {
        public NativeHashMap<int, CharacterStateData> StateData;
        public NativeHashMap<int, CharacterInputData> InputData;

        public bool IsEmpty() => StateData.IsEmpty && InputData.IsEmpty;

        public void Dispose() {
            StateData.Dispose();
            InputData.Dispose();
        }
    }
}