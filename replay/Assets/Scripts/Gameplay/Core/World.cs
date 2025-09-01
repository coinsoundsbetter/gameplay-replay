using System;
using System.Collections.Generic;

namespace Gameplay.Core {
    public class World {
        public readonly SystemGroup LogicRoot;
        public readonly SystemGroup FrameRoot;
        public readonly ActorManager ActorManager;

        public int WorldId { get; private set; }
        public string WorldName { get; private set; }
        public bool IsDisposed { get; private set; }
        
        private static int _nextWorldId = 1;
        private readonly Dictionary<Type, ISystem> _systemCache;

        public World(string worldName) {
            WorldId = _nextWorldId++;
            _systemCache = new Dictionary<Type, ISystem>();
            ActorManager = new ActorManager(WorldId);
            LogicRoot = new SystemGroup(this);
            FrameRoot = new SystemGroup(this);
            WorldName = worldName;
            IsDisposed = false;
            WorldRegistry.Register(this);
        }

        /// <summary>
        /// 驱动逻辑组（逻辑帧调用）
        /// </summary>
        public void TickLogic(double delta) {
            if (IsDisposed) return;
            var driverState = new SystemState()
            {
                DeltaTime = (float)delta,
            };
            LogicRoot.Update(ref driverState);
        }

        /// <summary>
        /// 驱动渲染组（渲染帧调用）
        /// </summary>
        public void TickFrame(float delta) {
            if (IsDisposed) return;
            var driverState = new SystemState()
            {
                DeltaTime = (float)delta,
            };
            FrameRoot.Update(ref driverState);
        }

        /// <summary>
        /// 释放整个 World，包括 Actor 和 System
        /// </summary>
        public void Dispose() {
            if (IsDisposed) return;

            // 系统清理
            var driverState = new SystemState();
            LogicRoot.OnDestroy(ref driverState);
            FrameRoot.OnDestroy(ref driverState);

            // 清理 Actor
            ActorManager.DestroyAllActors();

            WorldRegistry.Unregister(this);
            IsDisposed = true;
        }
        
        internal void RegisterSystem(ISystem sys) {
            _systemCache[sys.GetType()] = sys;
        }
        
        internal void UnregisterSystem(ISystem sys) {
            _systemCache.Remove(sys.GetType());
        }

        public T GetExistSystem<T>() where T : ISystem {
            if (_systemCache.TryGetValue(typeof(T), out var system)) {
                return (T)system;
            }

            return default;
        }
    }
}