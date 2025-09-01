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
            
            public void Serialize(Writer writer) {
                
            }

            public void Deserialize(Reader reader) {
                
            }
        }
    }
}