using System;
using Gameplay.Core;

namespace Gameplay.Client {
    
    [Order(SystemOrder.First)]
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public class NetworkInSystem : SystemBase {
        private NetworkClient client;
        
        protected override void OnCreate() {
            NetSync.OnClientReceived = OnReceived;
        }

        protected override void OnDestroy() {
            NetSync.OnClientReceived -= OnReceived;
        }
        
        private void OnReceived(ArraySegment<byte> data) {
            
        }
    }
}