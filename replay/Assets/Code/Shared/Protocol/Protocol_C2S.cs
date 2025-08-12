using FishNet.Serializing;
using UnityEngine;

namespace KillCam {
    public struct C2S_SendInput : INetworkMsg {
        public uint LocalTick;
        public Vector2Int Move;
        public float MouseX;

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
        public Quaternion Rotation;
        
        public byte[] Serialize(Writer writer) {
            writer.WriteQuaternion64(Rotation);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            Rotation = reader.ReadQuaternion64();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.C2S_SendCameraData;
    }

    public struct C2S_CmdFire : INetworkMsg, IBufferElement {
        public uint FireTick;
        public Vector3 FireOrigin;
        public Vector3 FireDir;
        public uint FireId;
        
        public byte[] Serialize(Writer writer) {
            writer.WriteUInt32(FireTick);
            writer.Write(FireOrigin);
            writer.Write(FireDir);
            writer.WriteUInt32(FireId);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            FireTick = reader.ReadUInt32();
            FireOrigin = reader.ReadVector3();
            FireDir = reader.ReadVector3();
            FireId = reader.ReadUInt32();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.CS2_CmdFire;
    }
}