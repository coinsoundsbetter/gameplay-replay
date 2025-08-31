using System;
using System.Collections.Generic;

namespace Gameplay.Core {
    public class ActorManager {
        private int _nextId = 1;
        private readonly HashSet<int> _aliveActors = new();
        private readonly Dictionary<Type, Dictionary<int, RefStorageBase>> _unmanaged = new();
        private readonly Dictionary<Type, Dictionary<int, object>> _managed = new();
        private readonly Dictionary<Type, Actor> _singletons = new();

        // ====================
        // Actor 生命周期
        // ====================
        public Actor CreateActor() {
            var actor = new Actor { Id = _nextId++ };
            _aliveActors.Add(actor.Id);
            return actor;
        }

        public void DestroyActor(Actor actor) {
            if (!_aliveActors.Remove(actor.Id))
                return;

            foreach (var map in _unmanaged.Values)
                map.Remove(actor.Id);

            foreach (var map in _managed.Values)
                map.Remove(actor.Id);
        }

        public IEnumerable<Actor> GetAllActors() {
            foreach (var id in _aliveActors)
                yield return new Actor { Id = id };
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
    }
}