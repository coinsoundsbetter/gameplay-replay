using FishNet.Serializing;
using UnityEngine;

namespace KillCam.Server {
    public class NetMessageHandle : Feature {
        
        protected override void OnCreate() {
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
                    OnC2S_SendInput(senderId, in msg1);
                    break;
                case NetworkMsg.CS2_CmdFire:
                    var msg2 = new C2S_CmdFire();
                    msg2.Deserialize(reader);
                    OnC2S_CmdFire(senderId, in msg2);
                    break;
            }
        }

        private void OnC2S_SendInput(int senderId, in C2S_SendInput message) {
            if (TryGetHero(senderId, out var hero)) {
                ref var ipData = ref hero.GetDataReadWrite<HeroInputData>();
                ipData.Move = message.Move; 
            }
        }
        
        private void OnC2S_CmdFire(int senderId, in C2S_CmdFire msg2) {
            if (TryGetHero(senderId, out var hero)) {
                ref var buffer = ref hero.GetBuffer<C2S_CmdFire>();
                buffer.Add(msg2);
            }
        }

        private bool TryGetHero(int senderId, out GameplayActor actor) {
            var roleMgr = GetWorldFeature<HeroManager>();
            if (roleMgr.RoleActors.TryGetValue(senderId, out actor)) {
                return true;
            }

            return false;
        }
    }
}