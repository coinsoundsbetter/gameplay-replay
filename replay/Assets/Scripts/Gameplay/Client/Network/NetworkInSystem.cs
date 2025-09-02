using System;
using FishNet.Serializing;
using Gameplay.Core;
using UnityEngine;

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
            var reader = new Reader(data, null);
            var msgId = reader.ReadInt32();
            var msgBody = NetMessageBootstrap.ClientRecv.CreateMessage(msgId);
            msgBody.Deserialize(reader);
            if (msgBody is Protocol.CreatePlayer) {
                Debug.Log("1");
            }
        }
    }
}