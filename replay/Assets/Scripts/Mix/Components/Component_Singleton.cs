using System.Collections.Generic;
using FishNet.Serializing;
using Unity.Entities;

namespace KillCam {
    
    public struct SingletonTag : IComponentData { }

    public struct GameIdPool : IComponentData {
        public int PlayerIdPool;
    }

    public class GameDict : IComponentData {
        public Dictionary<int, Entity> NetIdToNetState = new();
    }

    public class RpcQueue : IComponentData
    {
        public Queue<IServerRpc> RpcList = new();
        
        public void Add(IServerRpc serverRpcAll)
        {
            RpcList.Enqueue(serverRpcAll);
        }
     }

    public class SendQueue : IComponentData
    {
        public Queue<IClientSend> SendList = new();

        public void Add(IClientSend clientSend)
        {
            SendList.Enqueue(clientSend);
        }
    }

    public class NetChannels : IComponentData
    {
        public int LocalPlayerId;
        public Dictionary<int, FishNetChannel> Channels = new();
        
        public void Handle(IServerRpc serverRpc)
        {
            // TODO: Pooled
            var writer = new Writer();
            serverRpc.Serialize(writer);
            bool isRpcAll = serverRpc.TargetPlayerId == 0;
            if (isRpcAll)
            {
                foreach (var channel in Channels.Values)
                {
                    channel.Rpc(writer.GetBuffer());
                }
            }
            else
            {
                if (Channels.TryGetValue(serverRpc.TargetPlayerId, out var channel))
                {
                    channel.Rpc(writer.GetBuffer());
                }    
            }
        }

        public void Handle(IClientSend clientSend)
        {
            var writer = new Writer();
            clientSend.Serialize(writer);
            if (Channels.TryGetValue(LocalPlayerId, out var channel))
            {
                channel.Send(writer.GetBuffer());
            }
        }
    }
}