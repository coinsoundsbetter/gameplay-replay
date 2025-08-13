using System;
using FishNet.Serializing;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

namespace KillCam {
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
    
    public interface IBufferElement { }

    public interface INetworkContext {
        bool IsServer { get; }
        bool IsClient { get; }
        void SendToServer<T>(T message) where T : INetworkMsg;
        void SendToAllClients<T>(T message) where T : INetworkMsg;
        void SendToClient<T>(int playerId, T message) where T : INetworkMsg;
        uint GetTick();
        double GetNowTime();
        long GetRTT();
        long GetHalfRTT();
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

    public class RefStorageBuffer<T> : RefStorageBase where T : unmanaged, IBufferElement {
        public DynamicBuffer<T> Value;
        public ref DynamicBuffer<T> GetRef() => ref Unsafe.AsRef(in Value);

        public override void Dispose() {
            Value.Dispose();
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
        PostVisual,
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

    public struct FeatureState {
        public uint Tick;
        public double Delta;
        public bool IsPredictionTick;
    }

    public struct DynamicBuffer<T> : IDisposable where T : unmanaged, IBufferElement {
        private NativeList<T> buffer;

        public DynamicBuffer(int capacity, Allocator allocator) {
            buffer = new NativeList<T>(capacity, allocator);
        }

        public T this[int index] {
            get => buffer[index];
            set => buffer[index] = value;
        }

        public int Length => buffer.Length;

        public void Add(in T element) {
            buffer.Add(in element);
        }

        public void RemoveAt(int index) {
            buffer.RemoveAt(index);
        }

        public void Clear() {
            buffer.Clear();
        }

        public void Dispose() {
            if (buffer.IsCreated) {
                buffer.Dispose();
            }
        }
    }
}