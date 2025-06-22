using FishNet.Serializing;

namespace KillCam {
    public struct C2S_PlayerInputState : IClientSend
    {
        // 固定数据
        private const NetMsg Msg = NetMsg.CS2_PlayerInputState;
        
        // 自定义数据
        public PlayerInputState Data;
        
        public void Serialize(Writer writer)
        {
            writer.Write(Data.Move);
        }

        public void Deserialize(Reader reader)
        {
            Data = new PlayerInputState()
            {
                Move = reader.ReadVector2(),
            };
        }

        public NetMsg GetMsgType()
        {
            return Msg;
        }
    }
}