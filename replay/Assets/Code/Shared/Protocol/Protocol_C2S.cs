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

    public struct C2S_CmdFire : INetworkMsg {
        public uint ClientLocalTick;
        public Vector3 Origin;
        public Vector3 Direction;
        
        public byte[] Serialize(Writer writer) {
            writer.WriteUInt32(ClientLocalTick);
            writer.Write(Origin);
            writer.Write(Direction);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            ClientLocalTick = reader.ReadUInt32();
            Origin = reader.ReadVector3();
            Direction = reader.ReadVector3();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.C2S_SendProjectileFire;
    }
}