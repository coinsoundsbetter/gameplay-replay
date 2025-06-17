using Core;
using Unity.Collections;

namespace KillCam
{
    public struct RequestLoginGame : IComponent
    {
        public FixedString32Bytes UserName;
    }

    public struct PlayerTag : IComponent
    {
        public int Id;
        public FixedString32Bytes Name;
    }

    public struct PlayerHealth : IComponent
    {
        public int Hp;
        public int MaxHp;
    }

    public struct PlayerSpawnTag : IComponent
    {
        public int PlayerId;
        public FixedString32Bytes PlayerName;
    }

    public class PlayerNetConnection : IComponent
    {
        
    }
}