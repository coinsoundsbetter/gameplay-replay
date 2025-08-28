using System.Collections.Generic;

namespace KillCam {
    public abstract class Feature {
        private GameplayActor owner;
        private BattleWorld world;
        public bool IsActive { get; private set; }
        protected double TickDeltaDouble { get; private set; }
        protected float TickDelta => (float)TickDeltaDouble;

        public void Create(GameplayActor gameplayActor) {
            owner = gameplayActor;
            world = gameplayActor.MyWorld;
            IsActive = false;
            OnCreate();
        }

        public void Setup() {
            OnSetup();
        }

        public void Activate() {
            IsActive = true;
            OnActivate();
        }

        public void Deactivate() {
            IsActive = false;
            OnDeactivate();
        }

        public void Destroy() {
            if (IsActive) {
                OnDeactivate();
            }

            OnDestroy();
        }

        public void TickActive(double deltaTime) {
            TickDeltaDouble = deltaTime;
            OnTick();
        }

        protected virtual void OnCreate() { }

        protected virtual void OnDestroy() { }
        
        protected virtual void OnSetup() { }

        protected virtual void OnActivate() { }
        
        protected virtual void OnDeactivate() { }

        protected virtual void OnTick() { }

        public virtual bool OnShouldActivate() {
            return true;
        }
        
        //====================
        // Actor 级数据访问
        //====================

        protected void CreateData<T>() where T : unmanaged
            => world.SetupData(new T());
        
        protected bool HasData<T>() where T : unmanaged
            => world.HasActorData<T>(owner);

        protected bool HasDataManaged<T>() where T : class
            => world.HasActorDataManaged<T>(owner);

        protected bool TryGetDataManaged<T>(out T value) where T : class
            => world.TryGetActorDataManaged(owner, out value);

        protected ref T GetDataRef<T>() where T : unmanaged
            => ref world.GetActorDataRW<T>(owner);

        protected bool TryGetData<T>(out T value) where T : unmanaged {
            if (HasData<T>()) {
                value = GetDataRO<T>();
                return true;
            }

            value = default;
            return false;
        }

        protected ref readonly T GetDataRO<T>() where T : unmanaged
            => ref world.GetActorDataRO<T>(owner);

        protected void AddBuffer<T>() where T : unmanaged, IBufferElement
            => world.SetupActorBuffer<T>(owner);

        protected ref DynamicBuffer<T> GetBuffer<T>() where T : unmanaged, IBufferElement
            => ref world.GetActorBuffer<T>(owner);
        
        protected T GetDataManaged<T>() where T : class
            => world.GetActorDataManaged<T>(owner);

        protected GameplayActor CreateActor(ActorGroup actorGroup = ActorGroup.Default)
            => world.CreateActor(actorGroup);
        
        //====================
        // World 级数据访问
        //====================
        protected bool HasWorldData<T>() where T : unmanaged
            => world.HasData<T>();

        protected ref T GetWorldDataRW<T>() where T : unmanaged
            => ref world.GetDataRW<T>();

        protected ref readonly T GetWorldDataRO<T>() where T : unmanaged
            => ref world.GetDataRO<T>();

        protected T GetWorldDataManaged<T>() where T : class
            => world.GetDataManaged<T>();

        protected T GetWorldFeature<T>() where T : Feature
            => world.GetFeature<T>();

        protected ref DynamicBuffer<T> GetWorldBuffer<T>() where T : unmanaged, IBufferElement {
            return ref world.GetBuffer<T>();
        }
        
        protected bool HasWorldFlag(WorldFlag flag) => world.HasFlag(flag);
        
        //====================
        // 网络发送
        //====================
        protected void Send<T>(T msg) where T : INetworkMsg {
            world.NetworkContext.SendToServer(msg);
        }

        protected void BroadcastAll<T>(T msg) where T : INetworkMsg {
            world.NetworkContext.SendToAllClients(msg);
        }

        protected long GetRTT() {
            return world.NetworkContext.GetRTT();
        }

        protected long GetHalfRTT() {
            return world.NetworkContext.GetRTT();
        }

        protected double GetNetTime() {
            return world.NetworkContext.GetNowTime();
        }

        protected uint GetTick() => world.NetworkContext.GetTick();

        protected IEnumerable<GameplayActor> GetActors(ActorGroup group) {
            return world.GetActors(group);
        }
    }
}