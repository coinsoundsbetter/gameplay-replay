using Unity.Entities;

namespace KillCam {

    // 每个连接端,都有一个对应的实体拥有此组件
    public struct NetConnection : IComponentData {
        public int NetId;
        public int PlayerId;
        public NetConnectState NetState;
    }

    public struct ConnectState : IComponentData {
        
    }

    public enum NetConnectState {
        Unconnected,
        Connected,
        Disconnected,
    }
}