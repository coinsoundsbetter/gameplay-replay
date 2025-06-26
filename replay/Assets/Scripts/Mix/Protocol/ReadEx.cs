using FishNet.Serializing;

namespace KillCam
{
    public static class ReadEx
    {
        public static InputElement ReadInputElement(this Reader reader) {
            var data = new InputElement();
            data.Move = reader.ReadVector2();
            return data;
        }
    }
}