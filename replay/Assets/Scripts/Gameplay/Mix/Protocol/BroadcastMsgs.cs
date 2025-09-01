using FishNet.Broadcast;

namespace Gameplay {
    public struct LoginRequest : IBroadcast {
        public string PlayerName;
    }
}