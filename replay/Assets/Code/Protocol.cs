using FishNet.Serializing;
using UnityEngine;

public enum NetworkMsg : ushort
{
    C2S_SendInput,
}

public struct C2S_SendInput : INetworkSerialize
{
    public uint LocalTick;
    public Vector2 Move;
    
    public byte[] Serialize(Writer writer)
    {
        writer.WriteUInt32(LocalTick);
        writer.WriteVector2(Move);
        return writer.GetBuffer();
    }

    public void Deserialize(Reader reader)
    {
        LocalTick = reader.ReadUInt32();
        Move = reader.ReadVector2();
    }

    public NetworkMsg GetMsgType() => NetworkMsg.C2S_SendInput;
}