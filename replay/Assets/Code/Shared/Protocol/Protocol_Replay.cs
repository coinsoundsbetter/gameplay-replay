using System;
using System.Collections.Generic;
using FishNet.Serializing;

namespace KillCam {
    
    public struct S2C_Replay_WorldStateSnapshot : INetworkSerialize
    {
        public uint Tick;
        public Dictionary<int, RoleStateSnapshot> RoleStateSnapshots;

        public bool IsNull() => Tick == 0 && RoleStateSnapshots == null;
        
        public byte[] Serialize(Writer writer)
        {
            writer.Write(Tick);
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
            Tick = reader.ReadUInt32();
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