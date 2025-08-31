using System;

namespace Gameplay.Core {
    public struct Actor : IEquatable<Actor> {
        public int Id;

        public bool Equals(Actor other) {
            return Id == other.Id;
        }

        public override bool Equals(object obj) {
            return obj is Actor other && Equals(other);
        }

        public override int GetHashCode() {
            return Id;
        }
    }
}