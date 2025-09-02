using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;

namespace Gameplay {
    public class NetSync : NetworkBehaviour {
        public readonly SyncVar<int> Id = new SyncVar<int>();
        public static Action<NetSync> OnAddServer;
        public static Action<NetSync> OnRemoveServer;
        public static Action<NetSync> OnAddClient;
        public static Action<NetSync> OnRemoveClient;
        public static Action<ArraySegment<byte>> OnClientReceived;
        public static Action<int, ArraySegment<byte>> OnServerReceived;

        public override void OnStartClient() {
            OnAddClient?.Invoke(this);
        }

        public override void OnStopClient() {
            OnRemoveClient?.Invoke(this);
        }

        public override void OnStartServer() {
            OnAddServer?.Invoke(this);
        }

        public override void OnStartNetwork() {
            OnRemoveServer?.Invoke(this);
        }

        public void ClientRpc<T>(T message) where T : INetworkMessage {
            var writer = new Writer();
           // writer.WriteInt32(id);
            message.Serialize(writer);
            var byteArray = writer.GetArraySegment();
            RpcToServer(byteArray);
        }

        public void ServerRpc<T>(T message) where T : INetworkMessage {
            var msgId = NetMessageBootstrap.ClientRecv.GetId<T>();
            var writer = new Writer();
            writer.WriteInt32(msgId);
            message.Serialize(writer);
            var byteArray = writer.GetArraySegment();
            RpcToClient(byteArray);
        }

        [ServerRpc]
        private void RpcToServer(ArraySegment<byte> data) {
            OnServerReceived?.Invoke(Id.Value, data);
        }

        [ObserversRpc]
        private void RpcToClient(ArraySegment<byte> data) {
            OnClientReceived?.Invoke(data);
        }
    }
    
    public interface INetworkMessage {
        void Serialize(Writer w);
        void Deserialize(Reader r);
    }
}