using System.Collections.Generic;
using FishNet.Serializing;
using Unity.Entities;
using UnityEngine;

namespace KillCam {

    public struct NetTickState : IComponentData {
        public uint Local;
        public uint Server;
    }

    public struct PlayerInputState : IComponentData
    {
        public Vector2 Move;
    }
    
    public struct LocalConnectState : IComponentData
    {
        public NetConnectState State;
    }
    
    public enum NetConnectState {
        Undefined,
        Unconnected,
        Connected,
        Disconnected,
    }

    public class GameData : IComponentData
    {
        public int GameIdPool;
        public Dictionary<string, int> UserNameToPlayerId = new Dictionary<string, int>();
    }

    public class RpcQueue : IComponentData
    {
        public Queue<(IServerRpc, int)> RpcList = new();
        
        public void Add(IServerRpc serverRpcAll, int playerId = 0)
        {
            RpcList.Enqueue((serverRpcAll, playerId));
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
        
        public void Rpc(IServerRpc serverRpc, int playerId = 0)
        {
            // TODO: Pooled
            var writer = new Writer();
            // 写入消息类型
            writer.WriteUInt32((uint)serverRpc.GetMsgType());
            // 写入协议内容
            serverRpc.Serialize(writer);
            bool isRpcAll = playerId == 0;
            if (isRpcAll)
            {
                foreach (var channel in Channels.Values)
                {
                    channel.Rpc(writer.GetBuffer());
                }
            }
            else
            {
                if (Channels.TryGetValue(playerId, out var channel))
                {
                    channel.Rpc(writer.GetBuffer());
                }    
            }
        }

        public void Send(IClientSend clientSend)
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