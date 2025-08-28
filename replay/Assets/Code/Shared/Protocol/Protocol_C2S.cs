using FishNet.Serializing;
using Unity.Mathematics;
using UnityEngine;

namespace KillCam {
    public struct C2S_UserInputCmd : INetworkMsg {
        public uint LocalTick;
        public UserInputState InputState;

        public byte[] Serialize(Writer writer) {
            writer.WriteUInt32(LocalTick);
            writer.WriteVector2Int(InputState.Move);
            writer.WriteSingle(InputState.Pitch);
            writer.WriteSingle(InputState.Yaw);
            writer.WriteBoolean(InputState.IsFirePressed);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            LocalTick = reader.ReadUInt32();
            InputState = new UserInputState {
                Move = reader.ReadVector2Int(),
                Pitch = reader.ReadSingle(),
                Yaw = reader.ReadSingle(),
                IsFirePressed = reader.ReadBoolean()
            };
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
        public float3 FireOrigin;
        public float3 FireDir;
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
            FireOrigin = reader.Readfloat3();
            FireDir = reader.Readfloat3();
            FireId = reader.ReadUInt32();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.CS2_CmdFire;
    }
}