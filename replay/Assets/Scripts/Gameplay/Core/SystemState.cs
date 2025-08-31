using System;

namespace Gameplay.Core {
    public struct SystemState : IEquatable<SystemState> {
        public World World;
        public ActorManager ActorManager => World.ActorManager;
        public float DeltaTime;

        public bool Equals(SystemState other) {
            return Equals(World, other.World) && DeltaTime.Equals(other.DeltaTime);
        }

        public override bool Equals(object obj) {
            return obj is SystemState other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(World, DeltaTime);
        }
    }
}