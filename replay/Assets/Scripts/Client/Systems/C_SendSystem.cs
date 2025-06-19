using Unity.Entities;

namespace KillCam {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateAfter(typeof(PresentationSystemGroup))]
    public partial struct C_SendSystem : ISystem {
        
        public void OnUpdate(ref SystemState state) {
            
        }
    }
}