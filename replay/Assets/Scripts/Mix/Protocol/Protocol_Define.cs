using FishNet.Serializing;
using Unity.Entities;

namespace KillCam
{
    public interface INetMessage : IComponentData
    {
        void Serialize(Writer writer);
        void Deserialize(Reader reader);
        NetMsg GetMsgType();
    }

    public interface IClientSend : INetMessage { }

    public interface IServerRpc : INetMessage { }

    public struct S2CMsg
    {
        public byte[] Data;
    }

    public struct C2SMsg
    {
        public int PlayerId;
        public byte[] Data;
    }

    public struct NetMsgTag : IComponentData
    {
        public uint Tick;
    }
}