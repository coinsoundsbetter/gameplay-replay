namespace Gameplay.Core {
    public class World {
        public readonly SystemGroup LogicRoot;
        public readonly SystemGroup FrameRoot;
        public readonly ActorManager ActorManager = new ActorManager();

        public string WorldName { get; private set; }
        public bool IsDisposed { get; private set; }

        public World(string worldName) {
            LogicRoot = new SystemGroup(this);
            FrameRoot = new SystemGroup(this);
            WorldName = worldName;
            IsDisposed = false;
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
            LogicRoot.OnDestroy();
            FrameRoot.OnDestroy();

            // 清理 Actor
            foreach (var actor in ActorManager.GetAllActors())
                ActorManager.DestroyActor(actor);

            IsDisposed = true;
        }
    }
}