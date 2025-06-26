using System.Collections.Generic;
using FishNet.Serializing;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    
    public struct NetTickState : IComponentData {
        public uint Local;
        public uint Remote;
        public NetTickType TickType;
        public bool IsPredictTick;
        public bool IsRollbackTick;
    }

    public enum NetTickType
    {
        Normal = 0,
        Rollback = 1,
        Replaying = 2,
    }

    public struct InputElement : IBufferElementData {
        public float Delay;
        public uint LocalTick;
        public Vector2 Move;
        public bool IsAvailable;
        public bool IsApplyed;
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

    public class NetRpc : IComponentData
    {
        public Queue<(IServerRpc, int)> RpcList = new();
        
        public void Add(IServerRpc serverRpcAll, int playerId = 0)
        {
            RpcList.Enqueue((serverRpcAll, playerId));
        }
     }

    public class NetSend : IComponentData
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
        
        public bool IsChannelActive(int playerId) => Channels.ContainsKey(playerId);
        
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
            if (Channels.TryGetValue(LocalPlayerId, out var channel))
            {
                var writer = new Writer();
                // 客户端每次发送的时候,都会带上当前的服务器命令帧号
                writer.Write(channel.NetworkManager.TimeManager.Tick);
                writer.Write((uint)clientSend.GetMsgType());
                clientSend.Serialize(writer);
                channel.Send(writer.GetBuffer());
            }
        }
    }
}