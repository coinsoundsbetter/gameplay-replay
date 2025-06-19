using FishNet.Managing;
using Unity.Entities;

namespace KillCam {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(PresentationSystemGroup))]
    public partial class S_RpcSystem : SystemBase {
        private NetworkManager manager;
        
        public void OnCreate(ref SystemState state) {
          //  manager = NetworkManager.Singleton;
        }

        protected override void OnCreate() {
            
        }

        protected override void OnUpdate() {

        }
    }
}