using Unity.Entities;

namespace KillCam {
    internal class Server {
        private World world;

        internal void Init() {
            world = new World("server");
            var allSystemTypes = DefaultWorldInitialization.GetAllSystems(
                WorldSystemFilterFlags.ServerSimulation);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, allSystemTypes);
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
        }

        internal void Clear() {
            world.Dispose();
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
        }
    }
}