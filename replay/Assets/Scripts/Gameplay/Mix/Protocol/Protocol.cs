using System;
using FishNet.Serializing;
using UnityEngine;

namespace Gameplay {
    public class Protocol {

        public struct UserInputCmd {
            public uint Tick;
        }

        public struct CreatePlayer : INetworkMessage {
            public int PlayerId;
            public Vector3 Position;

            public int GetMessageTypeId() => StableHash(typeof(CreatePlayer));

            public void Serialize(Writer writer) {
                
            }

            public void Deserialize(Reader reader) {
                
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