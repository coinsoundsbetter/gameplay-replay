using System;

namespace KillCam {
    public struct GameplayActor : IEquatable<GameplayActor> {
        public bool IsActive;
        public int ActorID { get; private set; }
        public int WorldId { get; set; }
        public BattleWorld MyWorld => BattleWorld.Worlds[WorldId];

        public void Setup(int worldId, int actorId) {
            WorldId = worldId;
            ActorID = actorId;
            IsActive = true;
        }

        public void SetData<T>() where T : unmanaged {
            AddData<T>(new T());
        }

        public void AddData<T>(T instance) where T : unmanaged {
            MyWorld.AddActorData(this, instance);
        }

        public void SetDataManaged<T>(T instance) where T : class {
            MyWorld.AddActorDataManaged(this, instance);
        }

        public void SetupBuffer<T>() where T : unmanaged, IBufferElement {
            MyWorld.SetupActorBuffer<T>(this);
        }

        public ref DynamicBuffer<T> GetBuffer<T>() where T : unmanaged, IBufferElement {
            return ref MyWorld.GetActorBuffer<T>(this);
        }

        public bool HasData<T>() where T : unmanaged {
            return MyWorld.HasActorData<T>(this);
        }

        public ref T GetDataReadWrite<T>() where T : unmanaged {
            return ref MyWorld.GetActorDataRW<T>(this);
        }

        public T GetDataReadOnly<T>() where T : unmanaged {
            return MyWorld.GetActorDataRO<T>(this);
        }

        public T GetDataManaged<T>() where T : class {
            return MyWorld.GetActorDataManaged<T>(this);
        }

        public void AddFeature<T>(TickGroup tickGroup) where T : SystemBase, new() {
            MyWorld.AddActorFeature<T>(this, tickGroup);
        }

        public void AddFeature(SystemBase systemBase, TickGroup tickGroup) {
            MyWorld.AddActorFeature(this, systemBase, tickGroup);
        }

        public void SetupAllFeatures() {
            MyWorld.SetupAllActorFeatures(this);
        }

        public void Destroy() {
            MyWorld.DestroyActor(this);
        }

        public bool Equals(GameplayActor other) {
            return IsActive == other.IsActive && ActorID == other.ActorID && WorldId == other.WorldId;
        }

        public override bool Equals(object obj) {
            return obj is GameplayActor other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(IsActive, ActorID, WorldId);
        }
    }
}