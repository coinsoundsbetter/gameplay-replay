using System;
using System.Collections.Generic;
using FishNet.Serializing;
using System.Runtime.CompilerServices;
using FishNet.Managing;
using UnityEngine;

namespace KillCam {
    

    /// <summary>
    /// 客户端初始化
    /// </summary>
    

    /// <summary>
    /// 回放初始化
    /// </summary>
    

    /// <summary>
    /// 服务器初始化
    /// </summary>
    

    [System.Flags]
    public enum WorldFlag {
        Default = 0,
        Client = 1 << 0,
        Server = 1 << 1,
        Replay = 1 << 2,
    }

    public interface INetworkMsg {
        byte[] Serialize(Writer writer);
        void Deserialize(Reader reader);
        ushort GetMsgType();
    }

    public interface INetworkClient {
        void Send<T>(T message) where T : INetworkMsg;
        uint GetTick();
    }

    public interface INetworkServer {
        void Rpc<T>(T message) where T : INetworkMsg;
        void TargetRpc<T>(int id, T message) where T : INetworkMsg;
        uint GetTick();
    }

    public class RefStorageBase : IDisposable {
        public virtual void Dispose() {
        }
    }

    public class RefStorage<T> : RefStorageBase where T : unmanaged {
        public T Value;
        public ref T GetRef() => ref Unsafe.AsRef(in Value);

        public override void Dispose() {
            if (Value is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }

    public enum TickGroup {
        // 逻辑组
        NetworkReceive,
        PreSimulation,
        Input,
        Prediction,
        Simulation,
        CollisionAndHits,
        PostSimulation,
        
        // 视觉组
        Visual,
    }

    public enum ActorGroup {
        Default,
        Player,
        World,
    }

    public static class LayerDefine {
        public static readonly int defaultLayer = LayerMask.NameToLayer("Default");
        public static readonly int characterLayer = LayerMask.NameToLayer("Character");
        public static readonly int replayCharacterLayer = LayerMask.NameToLayer("ReplayCharacter");
    }
}