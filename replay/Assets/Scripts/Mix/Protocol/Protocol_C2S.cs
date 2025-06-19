using FishNet.Serializing;
using Unity.Collections;

namespace KillCam {
    public struct C2S_LoginGame : IClientSend {
        // 固定数据
        const NetMsg Msg = NetMsg.C2S_LoginGame;
        
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
}