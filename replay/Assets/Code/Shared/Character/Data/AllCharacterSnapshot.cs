using Unity.Collections;

namespace KillCam
{
    public struct AllCharacterSnapshot
    {
        public NativeHashMap<int, CharacterStateData> StateData;
        public NativeHashMap<int, CharacterInputData> InputData;
    }
}