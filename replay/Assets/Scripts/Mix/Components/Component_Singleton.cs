using System.Collections.Generic;
using Unity.Entities;

namespace KillCam {
    
    public struct SingletonTag : IComponentData { }

    public struct GameIdPool : IComponentData {
        public int PlayerIdPool;
    }

    public class GameDict : IComponentData {
        public Dictionary<int, Entity> NetIdToNetState = new();
    }
}