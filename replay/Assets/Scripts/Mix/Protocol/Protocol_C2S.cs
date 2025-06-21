using FishNet.Serializing;

namespace KillCam {
    public struct C2S_LoginGame : IClientSend {
        // 固定数据
        public const NetMsg Msg = NetMsg.C2S_LoginGame;
        
        // 自定义数据
        public string UserName;
        
        public void Serialize(Writer writer)
        {
            writer.Write(UserName);
        }

        public void Deserialize(Reader reader)
        {
            UserName = reader.ReadStringAllocated();
        }

        public NetMsg GetMsgType()
        {
            return Msg;
        }
    }

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