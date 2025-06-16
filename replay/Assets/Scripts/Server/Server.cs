using Unity.Entities;
using Unity.NetCode;

namespace KillCam {
    internal class Server {
        private World world;
        
        internal void Init() {
            world = ClientServerBootstrap.CreateServerWorld("server");
        }

        internal void Clear() {
            
        }
    }
}