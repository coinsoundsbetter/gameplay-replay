using FishNet.Serializing;

namespace KillCam
{
    public interface INetMessage
    {
        void Serialize(Writer writer);
        void Deserialize(Reader reader);
        NetMsg GetMsgType();
    }

    public interface IClientSend : INetMessage { }

    public interface IServerRpc : INetMessage { }

    public struct S2CMsg
    {
        public int TargetPlayerId;
        public byte[] Data;
    }

    public struct C2SMsg
    {
        public int PlayerId;
        public byte[] Data;
    }
}