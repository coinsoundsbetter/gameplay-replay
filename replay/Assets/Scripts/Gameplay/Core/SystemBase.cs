using System;

namespace Gameplay.Core {
    /// <summary>
    /// 面向继承的 class 版系统基类，带 OnStartRunning/OnStopRunning 生命周期
    /// </summary>
    public abstract class SystemBase : ISystem {
        protected World World { get; private set; }
        protected ActorManager Actors => World.ActorManager;
        protected float DeltaTime { get; private set; }

        private bool _hasStarted; // 是否已调用过 OnStartRunning
        private bool _isRunning; // 当前是否处于运行中

        public void OnCreate(ref SystemState state) {
            World = state.World;
            _hasStarted = false;
            _isRunning = false;
            OnCreate();
        }

        public void Update(ref SystemState state) {
            DeltaTime = state.DeltaTime;

            if (!_hasStarted) {
                _hasStarted = true;
                _isRunning = true;
                OnStartRunning();
            }

            if (_isRunning) {
                OnUpdate();
            }
        }

        public void OnDestroy(ref SystemState state) {
            if (_isRunning) {
                OnStopRunning();
                _isRunning = false;
            }
            OnDestroy();
        }

        // ========== 可重写的生命周期 ==========

        protected virtual void OnCreate() {
        }

        protected virtual void OnDestroy() {
            
        }

        /// <summary>系统第一次运行时调用</summary>
        protected virtual void OnStartRunning() {
        }

        /// <summary>每帧调用</summary>
        protected virtual void OnUpdate() {
        }

        /// <summary>系统停止运行（OnDestroy 前）</summary>
        protected virtual void OnStopRunning() {
        }
    }
}