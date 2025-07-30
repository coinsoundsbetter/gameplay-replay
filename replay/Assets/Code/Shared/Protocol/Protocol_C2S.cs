using System.Collections.Generic;
using FishNet.Serializing;
using UnityEngine;

namespace KillCam {
    public struct C2S_SendInput : INetworkMsg {
        public uint LocalTick;
        public Vector2Int Move;

        public byte[] Serialize(Writer writer) {
            writer.WriteUInt32(LocalTick);
            writer.WriteVector2Int(Move);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            LocalTick = reader.ReadUInt32();
            Move = reader.ReadVector2Int();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.C2S_SendInput;
    }

    public struct C2S_SendCameraData : INetworkMsg {
        public byte[] Serialize(Writer writer) {
            
        }

        public void Deserialize(Reader reader) {
            
        }

        public ushort GetMsgType() {
           
        }
    }
}