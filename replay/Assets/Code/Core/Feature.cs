namespace KillCam {
    public abstract class Feature {
        protected GameplayActor Owner { get; private set; }
        protected BattleWorld World { get; private set; }
        public bool IsActive { get; private set; }
        protected double TickDelta { get; private set; }

        public bool HasData<T>() where T : unmanaged {
            return World.HasActorData<T>(Owner);
        }

        public T GetDataRO<T>() where T : unmanaged {
            return World.GetActorDataRO<T>(Owner);
        }

        public ref T GetDataRW<T>() where T : unmanaged {
            return ref World.GetActorDataRW<T>(Owner);
        }

        public T GetDataManaged<T>() where T : class {
            return World.GetDataManaged<T>();
        }

        public void Setup(GameplayActor gameplayActor) {
            Owner = gameplayActor;
            World = gameplayActor.MyWorld;
            IsActive = false;
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

        protected virtual void OnSetup() { }

        protected virtual void OnDestroy() { }

        protected virtual void OnActivate() { }
        
        protected virtual void OnDeactivate() { }

        protected virtual void OnTickActive() { }

        public virtual bool OnShouldTick() {
            return true;
        }
    }
}