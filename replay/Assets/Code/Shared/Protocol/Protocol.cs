using System.Collections.Generic;
using FishNet.Serializing;
using UnityEngine;

namespace KillCam
{
    public enum NetworkMsg : ushort
    {
        C2S_SendInput,
        S2C_Replay_WorldStateSnapshot,
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

    public struct S2C_Replay_WorldStateSnapshot : INetworkSerialize
    {
        public Dictionary<int, RoleStateSnapshot> RoleStateSnapshots;
        
        public byte[] Serialize(Writer writer)
        {
            writer.Write(RoleStateSnapshots.Count);
            foreach (var kvp in RoleStateSnapshots)
            {
                writer.WriteInt32(kvp.Key);
                writer.WriteRoleStateSnapshot(kvp.Value);
            }

            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader)
        {
            RoleStateSnapshots = new Dictionary<int, RoleStateSnapshot>();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var id = reader.ReadInt32();
                var roleStateSnapshot = reader.ReadRoleSnapshot();
                RoleStateSnapshots.Add(id, roleStateSnapshot);
            }
        }

        public NetworkMsg GetMsgType()
        {
            return NetworkMsg.S2C_Replay_WorldStateSnapshot;
        }
    }
}