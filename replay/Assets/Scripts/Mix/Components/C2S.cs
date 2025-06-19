using Unity.Collections;
using Unity.Entities;

namespace KillCam {
    public struct C2S_LoginGame : IComponentData {
        public FixedString32Bytes UserName;
    }
}