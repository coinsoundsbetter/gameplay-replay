using System;
using FishNet.Serializing;

namespace KillCam {
    public struct S2C_WorldStateSnapshot : INetworkMsg, IDisposable {
        public uint Tick;
        public AllHeroSnapshot HeroSnapshot;

        public bool IsNull() => !HeroSnapshot.StateData.IsCreated &&
                                !HeroSnapshot.InputData.IsCreated;

        public byte[] Serialize(Writer writer) {
            writer.WriteUInt32(Tick);
            writer.WriteAllCharacterSnapshot(HeroSnapshot);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            Tick = reader.ReadUInt32();
            HeroSnapshot = reader.ReadAllCharacterSnapshot();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.S2C_WorldStateSnapshot;

        public void Dispose() {
            HeroSnapshot.Dispose();
        }
    }

    public struct S2C_StartReplay : INetworkMsg {
        public byte[] FullData;
        
        public byte[] Serialize(Writer writer) {
            writer.WriteInt32(FullData.Length);    
            writer.WriteUInt8Array(FullData, 0, FullData.Length);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            var len = reader.ReadInt32();
            FullData = new byte[len];
            reader.ReadUInt8Array(ref FullData, len);
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.S2C_StartReplay;
    }

    public struct S2C_FireAck : INetworkMsg {
        public uint FireId;
        public bool Accept;
        public uint ServerFireTick;
        public int HitHeroActorId;
        public ushort CauseDamage;
        
        public byte[] Serialize(Writer writer) {
            writer.WriteUInt32(FireId);
            writer.WriteBoolean(Accept);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            FireId = reader.ReadUInt32();
            Accept = reader.ReadBoolean();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.S2C_FireAck;
    }
}