using FishNet.Serializing;

namespace KillCam
{
    public static class WriteEx
    {
        public static void WriteRoleStateSnapshot(this Writer writer, RoleStateSnapshot data)
        {
            writer.WriteVector3(data.Pos);
            writer.WriteQuaternion64(data.Rot);
            writer.Write(data.Health);
            writer.Write(data.MoveInput);
        }
    }
}