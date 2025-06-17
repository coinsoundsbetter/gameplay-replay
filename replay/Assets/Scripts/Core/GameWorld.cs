using System;
using System.Collections.Generic;
using Arch.Core;
using KillCam;

namespace Core
{
    public class GameWorld : IGameLoop
    {
        private Dictionary<Type, SystemGroup> allSystemGroups = new();
        private InitializeSystemGroup initializeSystemGroup;
        private SimulationSystemGroup simulationSystemGroup;
        private FixedStepSystemGroup fixedStepSystemGroup;
        private PresentationSystemGroup presentationSystemGroup;

        private World ecsWorld;
        
        public T GetOrCreateSystemGroup<T>() where T : SystemGroup, new()
        {
            var type = typeof(T);
            if (allSystemGroups.TryGetValue(type, out var systemGroup))
            {
                return (T)systemGroup;
            }
            
            systemGroup = new T();
            systemGroup.SetEcsWorld(ecsWorld);
            allSystemGroups.Add(type, systemGroup);
            return (T)systemGroup;
        }

        public GameWorld()
        {
            ecsWorld = World.Create();
            initializeSystemGroup = GetOrCreateSystemGroup<InitializeSystemGroup>();
            simulationSystemGroup = GetOrCreateSystemGroup<SimulationSystemGroup>();
            fixedStepSystemGroup = GetOrCreateSystemGroup<FixedStepSystemGroup>();
            presentationSystemGroup = GetOrCreateSystemGroup<PresentationSystemGroup>();
        }

        public void Dispose()
        {
            presentationSystemGroup.RemoveAllSystems();
            simulationSystemGroup.RemoveAllSystems();
            fixedStepSystemGroup.RemoveAllSystems();
            initializeSystemGroup.RemoveAllSystems();
        }

        public void OnUpdate()
        {
            foreach (var s in initializeSystemGroup.Systems)
            {
                s.OnUpdate();
            }

            foreach (var s in simulationSystemGroup.Systems)
            {
                s.OnUpdate();
            }
        }

        public void OnLateUpdate()
        {
            foreach (var s in presentationSystemGroup.Systems)
            {
                s.OnUpdate();
            }
        }

        public void OnFixedUpdate()
        {
            foreach (var s in fixedStepSystemGroup.Systems)
            {
                s.OnUpdate();
            }
        }
    }
}