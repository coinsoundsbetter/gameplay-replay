using Gameplay.Core;

namespace Gameplay {
    public struct AddClientEvent : IEvent {
        public int Id;
        public bool IsLocalPlayer;
    }

    public struct RemoveClientEvent : IEvent {
        public int Id;
    }
}