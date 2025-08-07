using FishNet.Serializing;
using UnityEngine;

namespace KillCam.Server {
    public class NetMessageHandle : Feature {
        
        protected override void OnSetup() {
            HeroNet.OnServerReceiveData += HandleClientRequest;
        }

        protected override void OnDestroy() {
            HeroNet.OnServerReceiveData -= HandleClientRequest;
        }

        private void HandleClientRequest(int senderId, byte[] data) {
            var reader = new Reader(data, null);
            var msgType = (NetworkMsg)reader.ReadUInt16();
            switch (msgType) {
                case NetworkMsg.C2S_SendInput:
                    var msg1 = new C2S_SendInput();
                    msg1.Deserialize(reader);
                    OnC2S_SendInput(senderId, msg1);
                    break;
            }
        }

        private void OnC2S_SendInput(int senderId, C2S_SendInput message) {
            var roleMgr = World.GetFeature<HeroManager>();
            if (roleMgr.RoleActors.TryGetValue(senderId, out var actor)) {
                ref var ipData = ref actor.GetDataReadWrite<HeroInputData>();
                ipData.Move = message.Move;
            }
        }
    }
}