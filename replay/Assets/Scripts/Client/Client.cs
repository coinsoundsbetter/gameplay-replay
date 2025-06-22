using Unity.Entities;

namespace KillCam {
    internal class Client {
        private World world;

        internal void Init() {
            world = new World("client");
            var allSystemTypes = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.ClientSimulation);
            // 初始化系统确保最早创建
            world.GetOrCreateSystem<C_InitializeSystem>();
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, allSystemTypes);
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
        }

        internal void Clear() {
            world.Dispose();
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
        }
    }
}