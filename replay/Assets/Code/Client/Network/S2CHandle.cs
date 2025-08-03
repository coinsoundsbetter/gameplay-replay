using FishNet.Serializing;

namespace KillCam.Client {
    public class S2CHandle : Feature {
        public override void OnCreate() {
            HeroNet.OnClientReceiveData += OnReceived;
        }

        public override void OnDestroy() {
            HeroNet.OnClientReceiveData -= OnReceived;
        }

        private void OnReceived(byte[] data) {
            var reader = new Reader(data, null);
            var msgType = (NetworkMsg)reader.ReadUInt16();
            switch (msgType) {
                case NetworkMsg.S2C_StartReplay:
                    var startReplay = new S2C_StartReplay();
                    startReplay.Deserialize(reader);
                    OnHandle(startReplay);
                    break;
            }
        }

        private void OnHandle(S2C_StartReplay message) {
            ClientWorldsChannel.StartReplay(message.FullData);
        }
    }
}