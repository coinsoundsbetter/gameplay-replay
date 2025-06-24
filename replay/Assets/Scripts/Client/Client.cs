using Unity.Entities;

namespace KillCam {
    internal class Client {
        private World world;

        internal void Init() {
            world = new World("client");
            var allSystemTypes = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.ClientSimulation);
            // 初始化系统确保最早创建
            world.GetOrCreateSystem<C_InitializeSystem>();
            
            // 使用固定步长更新系统
            var fixedGroup = world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();
            
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, allSystemTypes);
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
        }

        internal void Clear() {
            world.Dispose();
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
        }
    }
}