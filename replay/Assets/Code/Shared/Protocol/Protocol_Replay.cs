using FishNet.Serializing;

namespace KillCam {
    public struct S2C_Replay_WorldStateSnapshot : INetworkSerialize {
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

        public ushort GetMsgType() => (ushort)NetworkMsg.S2C_Replay_WorldStateSnapshot;
    }
}