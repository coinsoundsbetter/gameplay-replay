using FishNet.Serializing;

namespace KillCam
{
    public static class ReadEx
    {
        public static PlayerInputState ReadPlayerInputState(this Reader reader)
        {
            var data = new PlayerInputState();
            data.Move = reader.ReadVector2();
            return data;
        }
    }
}