using Unity.Entities;
using Unity.NetCode;

namespace KillCam {
    internal class Client {
        private World world;

        internal void Init() {
            world = ClientServerBootstrap.CreateClientWorld("client");
        }

        internal void Clear() {
            
        }
    }
}