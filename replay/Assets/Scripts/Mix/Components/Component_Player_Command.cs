using Unity.NetCode;

namespace Mix
{
    public struct CmdPlayerInput : IRpcCommand {
        public uint PlayerId;
        public PlayerInput Input;
    }
}