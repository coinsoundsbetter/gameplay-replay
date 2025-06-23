using FishNet.Serializing;
using Unity.Collections;
using UnityEngine;

namespace KillCam {

    public enum NetMsg : uint
    {
        C2S = 0,
        CS2_PlayerInputState,
        
        S2C = 10000,
        S2C_SpawnPlayer,
    }
    
    public struct S2C_NetSpawnPlayer : IServerRpc
    {
        // 消息类型
        const NetMsg Msg = NetMsg.S2C_SpawnPlayer;
        
        // 自定义部分
        public int PlayerId;
        public FixedString32Bytes PlayerName;
        public Vector3 Pos;
        public Quaternion Rot;
        
        public void Serialize(Writer writer)
        {
            writer.Write(PlayerId);
            writer.WriteFixedString32Bytes(PlayerName);
            writer.WriteVector3(Pos);
            writer.Write(Rot);
        }

        public void Deserialize(Reader reader)
        {
            PlayerId = reader.ReadInt32();
            PlayerName = reader.ReadFixedString32Bytes();
            Pos = reader.ReadVector3();
            Rot = reader.Read<Quaternion>();
        }
        
        public NetMsg GetMsgType() => Msg;
    }

    
}