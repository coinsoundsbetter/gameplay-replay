using System;

namespace Gameplay.Core {
    public struct Actor : IEquatable<Actor> {
        public int Id;
        public int WorldId;
        
        public bool Equals(Actor other) {
            return Id == other.Id && WorldId == other.WorldId;
        }

        public override bool Equals(object obj) {
            return obj is Actor other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Id, WorldId);
        }
    }
}