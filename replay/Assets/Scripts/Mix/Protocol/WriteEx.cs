using FishNet.Serializing;

namespace KillCam
{
    public static class WriteEx
    {
        public static void WriteInputElement(this Writer writer, InputElement data)
        {
            writer.WriteVector2(data.Move);
        }
    }
}