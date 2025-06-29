using FishNet.Serializing;

namespace KillCam.Server
{
    public class BaseFeature_C2SHandle : Feature
    {
        public override void OnCreate()
        {
            RoleNet.OnServerReceiveData += HandleClientRequest;
        }

        public override void OnDestroy()
        {
            RoleNet.OnServerReceiveData -= HandleClientRequest;
        }

        private void HandleClientRequest(int senderId, byte[] data)
        {
            var reader = new Reader(data, null);
            var msgType = (NetworkMsg)reader.ReadUInt16();
            switch (msgType)
            {
                case NetworkMsg.C2S_SendInput:
                    var msg1 = new C2S_SendInput();
                    msg1.Deserialize(reader);
                    OnC2S_SendInput(senderId, msg1);
                    break;
            }
        }

        private void OnC2S_SendInput(int senderId, C2S_SendInput message)
        {
            var feature = world.Get<BaseFeature_ServerSpawn>();
            if (feature.TryGetRole(senderId, out var role))
            {
                role.Input.AddInput(message);
            }
        }
    }
}