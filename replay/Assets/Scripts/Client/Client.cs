using Unity.Entities;

namespace KillCam {
    internal class Client {
        private World world;

        internal void Init() {
            world = new World("client");
            var allSystemTypes = DefaultWorldInitialization.GetAllSystems(
                WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.Default);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, allSystemTypes);
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
        }

        internal void Clear() {
            world.Dispose();
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
        }
    }
}