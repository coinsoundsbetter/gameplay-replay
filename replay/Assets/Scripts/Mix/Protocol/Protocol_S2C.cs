using FishNet.Serializing;
using Unity.Collections;
using UnityEngine;

namespace KillCam {

    public enum NetMsg
    {
        C2S = 0,
        C2S_LoginGame,
        
        S2C = 10000,
        S2C_NetSpawnPlayer,
    }
    
    public struct S2C_NetSpawnPlayer : IServerRpc
    {
        // 固定数据
        const NetMsg Msg = NetMsg.C2S;
        public int TargetPlayerId { get; set; }
        
        // 自定义部分
        public int PlayerId;
        public FixedString32Bytes PlayerName;
        public Vector3 Pos;
        public Quaternion Rot;
        
        public void Serialize(Writer writer)
        {
            writer.Write(PlayerId);
            writer.Write(PlayerName);
            writer.WriteVector3(Pos);
            writer.Write(Rot);
        }

        public void Deserialize(Reader reader)
        {
            
        }
        
        public NetMsg GetMsgType() => Msg;
    }

    
}