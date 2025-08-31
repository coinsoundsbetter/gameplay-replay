using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FishNet.Serializing;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

namespace KillCam {
    [System.Flags]
    public enum WorldFlag {
        Default = Client | Server | Replay,
        Client = 1 << 0,
        Server = 1 << 1,
        Replay = 1 << 2,
    }

    public interface INetworkMsg {
        byte[] Serialize(Writer writer);
        void Deserialize(Reader reader);
        ushort GetMsgType();
    }

    public interface IBufferElement {
    }

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

        public virtual object GetBoxed() {
            return null;
        }
    }

    public class RefStorage<T> : RefStorageBase where T : unmanaged {
        public T Value;
        public ref T GetRef() => ref Unsafe.AsRef(in Value);

        public override object GetBoxed() {
            return Value;
        }

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
        Bullet,
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateInGroupAttribute : Attribute {
        public Type GroupType { get; }
        public UpdateInGroupAttribute(Type groupType) => GroupType = groupType;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateBeforeAttribute : Attribute {
        public Type TargetType { get; }
        public UpdateBeforeAttribute(Type targetType) => TargetType = targetType;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateAfterAttribute : Attribute {
        public Type TargetType { get; }
        public UpdateAfterAttribute(Type targetType) => TargetType = targetType;
    }

    public interface ISystem {
        void Create(BattleWorld world);
        void Destroy();
        void Tick(double delta);
    }

    public class SystemGroup : ISystem {
        private readonly List<ISystem> _systems = new List<ISystem>();
        public IReadOnlyList<ISystem> Systems => _systems;

        private BattleWorld myWorld;
        
        public void AddSystem(ISystem sys)
        {
            _systems.Add(sys);
            sys.Create(myWorld);
        }

        public void SortSystems() {
            var typeMap = _systems.ToDictionary(s => s.GetType(), s => s);
            var graph = new Dictionary<Type, List<Type>>();

            // 建图
            foreach (var sys in _systems) {
                var t = sys.GetType();
                if (!graph.ContainsKey(t)) graph[t] = new List<Type>();

                foreach (var after in t.GetCustomAttributes(typeof(UpdateAfterAttribute), true)
                             .Cast<UpdateAfterAttribute>()) {
                    if (typeMap.ContainsKey(after.TargetType))
                        graph[t].Add(after.TargetType);
                }

                foreach (var before in t.GetCustomAttributes(typeof(UpdateBeforeAttribute), true)
                             .Cast<UpdateBeforeAttribute>()) {
                    if (typeMap.ContainsKey(before.TargetType)) {
                        if (!graph.ContainsKey(before.TargetType))
                            graph[before.TargetType] = new List<Type>();
                        graph[before.TargetType].Add(t);
                    }
                }
            }

            // 拓扑排序
            var sorted = TopologicalSort(graph);
            _systems.Sort((a, b) => sorted.IndexOf(a.GetType()) - sorted.IndexOf(b.GetType()));
        }

        public void Create(BattleWorld world) {
            myWorld = world;
        }

        public void Destroy() {
            foreach (var system in _systems) {
                system.Destroy();
            }
            _systems.Clear();
        }

        public void Tick(double delta) {
            foreach (var sys in _systems)
                sys.Tick(delta);
        }

        private static List<Type> TopologicalSort(Dictionary<Type, List<Type>> graph) {
            var result = new List<Type>();
            var visited = new HashSet<Type>();
            var temp = new HashSet<Type>();

            void Visit(Type t) {
                if (visited.Contains(t)) return;
                if (temp.Contains(t)) throw new Exception("Cyclic dependency detected!");
                temp.Add(t);
                if (graph.TryGetValue(t, out var deps))
                    foreach (var d in deps)
                        Visit(d);
                temp.Remove(t);
                visited.Add(t);
                result.Add(t);
            }

            foreach (var node in graph.Keys)
                Visit(node);

            return result;
        }
    }

    public class LogicTickSystemGroup : SystemGroup {
    }

    public class FrameTickSystemGroup : SystemGroup {
    }

    public static class SystemCollector {
        /// <summary>
        /// 将所有属于 T 组的用户系统收集并添加到已有的 group 中。
        /// 已经手动添加过的系统不会重复加入。
        /// </summary>
        public static void CollectInto<T>(T group, WorldFlag world) where T : SystemGroup
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allTypes = allAssemblies
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null)!; }
                })
                .Where(t => typeof(ISystem).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in allTypes)
            {
                // 禁止收集的系统
                if (type.GetCustomAttribute<NeedManualCreateAttribute>() != null)
                    continue;

                // WorldFlag 检查
                var worldAttr = type.GetCustomAttribute<WorldFilterAttribute>();
                if (worldAttr != null && (worldAttr.Flag & world) == 0)
                    continue;

                // UpdateInGroup 检查
                var groupAttrs = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true)
                    .Cast<UpdateInGroupAttribute>();

                if (groupAttrs.Any(a => a.GroupType == typeof(T)))
                {
                    // 跳过已经存在的
                    if (group.Systems.Any(s => s.GetType() == type))
                        continue;

                    var instance = (ISystem)Activator.CreateInstance(type)!;
                    group.AddSystem(instance);
                }
            }

            group.SortSystems();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WorldFilterAttribute : Attribute {
        public WorldFlag Flag { get; }
        public WorldFilterAttribute(WorldFlag flag) => Flag = flag;
    }
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NeedManualCreateAttribute : Attribute { }
}