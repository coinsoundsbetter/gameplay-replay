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

    public interface IServerRpc : INetMessage
    {
        int TargetPlayerId { get; set; }
    }
}