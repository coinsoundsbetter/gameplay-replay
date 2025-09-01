using System;
using System.Collections.Generic;

namespace Gameplay {
    
    public class NetworkMsgRegistry {
        public static NetworkMsgRegistry Instance;
        private Dictionary<Type, int> msgIndies = new();
        
        public void Init() {
                
        }

        private void Register<T>(int msgTypeId) where T : INetworkMessage {
            
        }

        public int GetMsgTypeId<T>() where T : INetworkMessage {
            return 0;
        }

        public bool TryGetMsgTypeId<T>(out int msgTypeId) where T : INetworkMessage {
            msgTypeId = 0;
            return false;
        }
    }
}