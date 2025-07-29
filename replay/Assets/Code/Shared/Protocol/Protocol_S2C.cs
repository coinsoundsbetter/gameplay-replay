using System;
using FishNet.Serializing;

namespace KillCam {
    public struct S2C_WorldStateSnapshot : INetworkSerialize, IDisposable {
        public uint Tick;
        public AllCharacterSnapshot CharacterSnapshot;

        public bool IsNull() => !CharacterSnapshot.StateData.IsCreated &&
                                !CharacterSnapshot.InputData.IsCreated;

        public byte[] Serialize(Writer writer) {
            writer.WriteUInt32(Tick);
            writer.WriteAllCharacterSnapshot(CharacterSnapshot);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader) {
            Tick = reader.ReadUInt32();
            CharacterSnapshot = reader.ReadAllCharacterSnapshot();
        }

        public ushort GetMsgType() => (ushort)NetworkMsg.S2C_WorldStateSnapshot;

        public void Dispose() {
            CharacterSnapshot.Dispose();
        }
    }

    public struct S2C_StartReplay : INetworkSerialize {
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
}