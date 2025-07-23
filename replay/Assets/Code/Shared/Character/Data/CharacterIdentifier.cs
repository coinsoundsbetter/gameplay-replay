using Unity.Collections;

namespace KillCam
{
    public struct CharacterIdentifier
    {
        public bool IsControlTarget;
        public int PlayerId;
        public FixedString64Bytes PlayerName;
    }
}