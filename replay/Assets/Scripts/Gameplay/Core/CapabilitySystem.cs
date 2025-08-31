namespace Gameplay.Core {
    
    public class CapabilitySystem<T> : SystemBase where T : Capability {
        public T DeclaringType;
        
        protected override void OnUpdate()
        {
            foreach (var actor in World.ActorManager.GetAllActors())
            {
                foreach (var cap in World.ActorManager.GetCapabilities(actor))
                {
                    if (cap is T t && t.Enabled && t.SystemGroupType == typeof(T))
                    {
                        t.Tick(DeltaTime);
                    }
                }
            }
        }
    }
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlayerNormalLogicGroup : CapabilitySystem<Capability> { }
    
    [UpdateInGroup(typeof(VisualizeSystemGroup))]
    public class PlayerNormalVisualGroup : CapabilitySystem<Capability> { }
}