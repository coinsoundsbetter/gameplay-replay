using FishNet.Serializing;

namespace KillCam
{
    public static class WriteEx
    {
        public static void WritePlayerInputState(this Writer writer, PlayerInputState data)
        {
            writer.WriteVector2(data.Move);
        }
    }
}