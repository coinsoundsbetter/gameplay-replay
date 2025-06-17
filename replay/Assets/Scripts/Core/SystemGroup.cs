using System.Collections.Generic;
using Arch.Core;

namespace Core
{
    public class SystemGroup
    {
        public List<ISystem> Systems = new();
        private World ecs;

        public void SetEcsWorld(World world)
        {
            ecs = world;
        }
        
        public void AddSystem<T>() where T : ISystem, new()
        {
            var system = new T
            {
                ECS = ecs
            };
            Systems.Add(system);
            system.OnCreate();
        }

        public void RemoveAllSystems()
        {
            var len = Systems.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                var system = Systems[i];
                system.OnDestroy();
            }
            Systems.Clear();
        }
    }
}