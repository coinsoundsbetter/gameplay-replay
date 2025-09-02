using System;
using FishNet.Serializing;
using UnityEngine;

namespace Gameplay {
    public class Protocol {

        public struct UserInputCmd {
            public uint Tick;
        }

        [MessageId(1001)]
        public struct CreatePlayer : INetworkMessage {
            public int PlayerId;
            public Vector3 Position;

            public void Serialize(Writer w) {
                w.WriteInt32(PlayerId);
                w.WriteVector3(Position);
            }

            public void Deserialize(Reader r) {
                PlayerId = r.ReadInt32();
                Position = r.ReadVector3();
            }
        }
        
        public static int StableHash(Type type) {
            var str = type.FullName;
            unchecked {
                const uint fnvPrime = 16777619;
                uint hash = 2166136261;
                foreach (var c in str) {
                    hash ^= c;
                    hash *= fnvPrime;
                }
                return (int)hash;
            }
        }
    }
}