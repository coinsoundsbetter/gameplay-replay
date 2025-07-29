using System.Collections.Generic;
using FishNet.Serializing;
using UnityEngine;

namespace KillCam {
    public enum NetworkMsg : ushort {
        C2S_SendInput,


        S2C_Replay_WorldStateSnapshot,
        S2C_Replay_SpawnState,
    }

    public struct C2S_SendInput : INetworkSerialize {
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
}