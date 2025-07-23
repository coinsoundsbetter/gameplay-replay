using System;
using System.Collections.Generic;
using FishNet.Serializing;
using Unity.Collections;

namespace KillCam {
    
    public struct S2C_Replay_WorldStateSnapshot : INetworkSerialize
    {
        public uint Tick;
        public AllCharacterSnapshot CharacterSnapshot; 

        public bool IsNull() => !CharacterSnapshot.StateData.IsCreated &&
                                !CharacterSnapshot.InputData.IsCreated;
        
        public byte[] Serialize(Writer writer)
        {
            writer.WriteUInt32(Tick);
            writer.WriteAllCharacterSnapshot(CharacterSnapshot);
            return writer.GetBuffer();
        }

        public void Deserialize(Reader reader)
        {
            Tick = reader.ReadUInt32();
            CharacterSnapshot = reader.ReadAllCharacterSnapshot();
        }

        public NetworkMsg GetMsgType()
        {
            return NetworkMsg.S2C_Replay_WorldStateSnapshot;
        }
    }
}