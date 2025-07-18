using FishNet.Serializing;

namespace KillCam
{
    public static class ReadEx
    {
        public static RoleStateSnapshot ReadRoleSnapshot(this Reader reader)
        {
            var data = new RoleStateSnapshot
            {
                Pos = reader.ReadVector3(),
                Rot = reader.ReadQuaternion64(),
                Health = reader.ReadInt32(),
                MoveInput = reader.ReadVector2Int(),
            };
            return data;
        }
    }
}