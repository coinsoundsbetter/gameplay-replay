using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Gameplay.Core {
    public class ActorManager {
        private int _nextId = 1;
        private int _worldId;
        private readonly HashSet<Actor> _aliveActors = new();
        private readonly Dictionary<Type, Dictionary<int, RefStorageBase>> _unmanaged = new();
        private readonly Dictionary<Type, Dictionary<int, object>> _managed = new();
        private readonly Dictionary<Type, Actor> _singletons = new();
        private readonly Dictionary<Type, object> _managedSingletons = new();
        private readonly Dictionary<int, List<Capability>> _actorCapabilities = new();
        private readonly Dictionary<Type, RefStorageBase> _bufferSingletons = new();

        public ActorManager(int worldId) {
            _worldId = worldId;
        }

        ~ActorManager() {
            foreach (var kvp in _unmanaged.Values) {
                foreach (var dataWrap in kvp.Values) {
                    dataWrap.Dispose();
                }
            }
            foreach (var bufferWrap in _bufferSingletons.Values) {
                bufferWrap.Dispose();
            }
        }

        // ====================
        // Actor 生命周期
        // ====================
        public Actor CreateActor() {
            var actor = new Actor { Id = _nextId++, WorldId = _worldId};
            _aliveActors.Add(actor);
            return actor;
        }

        public void DestroyActor(Actor actor) {
            if (!_aliveActors.Remove(actor))
                return;

            foreach (var map in _unmanaged.Values)
                map.Remove(actor.Id);

            foreach (var map in _managed.Values)
                map.Remove(actor.Id);
        }
        
        public void DestroyAllActors()
        {
            foreach (var actor in _aliveActors.ToArray()) // 或用 ToList()
                DestroyActor(actor);
        }

        // ====================
        // 非托管数据（struct）
        // ====================
        public void AddData<T>(Actor actor, T value) where T : unmanaged {
            var type = typeof(T);
            if (!_unmanaged.ContainsKey(type)) {
                _unmanaged.Add(type, new Dictionary<int, RefStorageBase>());
            }

            _unmanaged[type].Add(actor.Id, new RefStorage<T>() {
                Value = value,
            });
        }

        public ref T GetDataRW<T>(Actor actor) where T : unmanaged {
            var type = typeof(T);
            var unmanaged = _unmanaged[type];
            return ref ((RefStorage<T>)unmanaged[actor.Id]).GetRef();
        }

        public ref readonly T GetDataRO<T>(Actor actor) where T : unmanaged {
            var type = typeof(T);
            var unmanaged = _unmanaged[type];
            return ref ((RefStorage<T>)unmanaged[actor.Id]).GetRef();
        }

        public bool HasData<T>(Actor actor) where T : unmanaged
            => _unmanaged.TryGetValue(typeof(T), out var map) && map.ContainsKey(actor.Id);

        public void RemoveData<T>(Actor actor) where T : unmanaged {
            if (_unmanaged.TryGetValue(typeof(T), out var map))
                map.Remove(actor.Id);
        }

        // ====================
        // 托管数据（class）
        // ====================
        public void AddDataManaged<T>(Actor actor, T value) where T : class {
            if (!_managed.TryGetValue(typeof(T), out var map)) {
                map = new Dictionary<int, object>();
                _managed[typeof(T)] = map;
            }

            map[actor.Id] = value;
        }

        public T GetDataManaged<T>(Actor actor) where T : class {
            return (T)_managed[typeof(T)][actor.Id];
        }

        public bool HasDataManaged<T>(Actor actor) where T : class
            => _managed.TryGetValue(typeof(T), out var map) && map.ContainsKey(actor.Id);

        public void RemoveDataManaged<T>(Actor actor) where T : class {
            if (_managed.TryGetValue(typeof(T), out var map))
                map.Remove(actor.Id);
        }

        // ====================
        // 单例支持
        // ====================
        public Actor CreateSingleton<T>(T data) where T : unmanaged {
            if (_singletons.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Singleton {typeof(T).Name} 已存在");

            var actor = CreateActor();
            AddData(actor, data);
            _singletons[typeof(T)] = actor;
            return actor;
        }

        public ref T GetSingleton<T>() where T : unmanaged {
            if (!_singletons.TryGetValue(typeof(T), out var actor))
                throw new InvalidOperationException($"Singleton {typeof(T).Name} 不存在");

            return ref GetDataRW<T>(actor);
        }

        public bool HasSingleton<T>() where T : unmanaged
            => _singletons.ContainsKey(typeof(T));

        public void DestroySingleton<T>() where T : unmanaged {
            if (_singletons.TryGetValue(typeof(T), out var actor)) {
                DestroyActor(actor);
                _singletons.Remove(typeof(T));
            }
        }

        // 创建一个新的托管单例并返回
        public T CreateSingletonManaged<T>() where T : class, new() {
            var type = typeof(T);
            if (_managedSingletons.ContainsKey(type))
                throw new InvalidOperationException($"Singleton {type.Name} already exists.");

            var instance = new T();
            _managedSingletons[type] = instance;
            return instance;
        }

        // 添加外部传入的托管单例
        public void AddSingletonManaged<T>(T instance) where T : class {
            var type = typeof(T);
            if (_managedSingletons.ContainsKey(type))
                throw new InvalidOperationException($"Singleton {type.Name} already exists.");
            _managedSingletons[type] = instance;
        }

        // 获取托管单例
        public T GetSingletonManaged<T>() where T : class {
            if (_managedSingletons.TryGetValue(typeof(T), out var obj))
                return (T)obj;
            throw new KeyNotFoundException($"Singleton {typeof(T).Name} not found.");
        }

        // 尝试获取托管单例
        public bool TryGetSingletonManaged<T>(out T value) where T : class {
            if (_managedSingletons.TryGetValue(typeof(T), out var obj)) {
                value = (T)obj;
                return true;
            }

            value = null!;
            return false;
        }

        // 移除托管单例
        public void RemoveSingletonManaged<T>() where T : class {
            _managedSingletons.Remove(typeof(T));
        }

        public IEnumerable<Actor> GetAllActors() => _aliveActors;
        
        /// <summary>
        /// 获取某一类Actor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Actor> GetActorsWith<T>() where T : unmanaged {
            if (_unmanaged.TryGetValue(typeof(T), out var map))
            {
                foreach (var id in map.Keys)
                {
                    yield return new Actor { Id = id };
                }
            }
        }
        
        // ====================
        // 动态缓冲区支持
        // ====================
        public void CreateSingletonBuffer<T>() where T : unmanaged, IBufferElement {
            var type = typeof(T);
            _bufferSingletons.Add(type, new RefStorageBuffer<T>() {
                Value = new DynamicBuffer<T>(2, Allocator.Persistent),
            });
        }

        public ref DynamicBuffer<T> GetSingletonBuffer<T>() where T : unmanaged, IBufferElement {
            var type = typeof(T);
            if (_bufferSingletons.TryGetValue(type, out var bufferWrapper)) {
                return ref ((RefStorageBuffer<T>)bufferWrapper).GetRef();
            }
            
            throw new KeyNotFoundException($"No dynamic buffer of type {type}");
        }
        
        // ====================
        // 能力组件支持
        // ====================
        
        /// <summary>
        /// 一次性为 Actor 添加一组能力，并指定 SystemGroup
        /// </summary>
        public void AddCapabilities(Actor actor, Type systemGroupType, params Capability[] caps)
        {
            if (!_actorCapabilities.TryGetValue(actor.Id, out var list))
            {
                list = new List<Capability>();
                _actorCapabilities[actor.Id] = list;
            }

            foreach (var cap in caps)
            {
                cap.Attach(actor, systemGroupType);
                list.Add(cap);
            }
        }

        /// <summary>
        /// 获取指定类型的能力
        /// </summary>
        public T GetCapability<T>(Actor actor) where T : Capability
        {
            if (_actorCapabilities.TryGetValue(actor.Id, out var list))
            {
                foreach (var cap in list)
                    if (cap is T t) return t;
            }
            return null!;
        }

        /// <summary>
        /// 获取 Actor 的所有能力
        /// </summary>
        public IEnumerable<Capability> GetCapabilities(Actor actor)
        {
            return _actorCapabilities.TryGetValue(actor.Id, out var list)
                ? list
                : Array.Empty<Capability>();
        }

        /// <summary>
        /// 移除所有能力（彻底卸载）
        /// </summary>
        public void RemoveAllCapabilities(Actor actor)
        {
            if (_actorCapabilities.TryGetValue(actor.Id, out var list))
            {
                foreach (var cap in list)
                    cap.Detach();
                list.Clear();
            }
        }
    }
}