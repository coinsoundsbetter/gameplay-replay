namespace KillCam {
    public abstract class Feature {
        protected GameplayActor Owner { get; private set; }
        private BattleWorld world;
        public bool IsActive { get; private set; }
        protected double TickDelta { get; private set; }
        
        protected float TickDeltaFloat => (float)TickDelta;

        public void Create(GameplayActor gameplayActor) {
            Owner = gameplayActor;
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
            TickDelta = deltaTime;
            OnTickActive();
        }

        protected virtual void OnCreate() { }

        protected virtual void OnDestroy() { }
        
        protected virtual void OnSetup() { }

        protected virtual void OnActivate() { }
        
        protected virtual void OnDeactivate() { }

        protected virtual void OnTickActive() { }

        public virtual bool OnShouldActivate() {
            return true;
        }
        
        //====================
        // Actor 级数据访问
        //====================

        protected void SetupData<T>() where T : unmanaged
            => world.SetupData(new T());
        
        protected bool HasData<T>() where T : unmanaged
            => world.HasActorData<T>(Owner);

        protected bool HasDataManaged<T>() where T : class
            => world.HasActorDataManaged<T>(Owner);

        protected bool TryGetDataManaged<T>(out T value) where T : class
            => world.TryGetActorDataManaged(Owner, out value);

        protected ref T GetDataRW<T>() where T : unmanaged
            => ref world.GetActorDataRW<T>(Owner);

        protected ref readonly T GetDataRO<T>() where T : unmanaged
            => ref world.GetActorDataRO<T>(Owner);

        protected ref DynamicBuffer<T> GetBuffer<T>() where T : unmanaged, IBufferElement
            => ref world.GetActorBuffer<T>(Owner);
        
        protected T GetDataManaged<T>() where T : class
            => world.GetActorDataManaged<T>(Owner);

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
    }
}