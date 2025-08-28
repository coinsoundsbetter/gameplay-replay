namespace KillCam {
    public class GameplayActor {
        public BattleWorld MyWorld { get; private set; }

        public void SetupWorld(BattleWorld world) {
            MyWorld = world;
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

        public void AddFeature<T>(TickGroup tickGroup) where T : Feature, new() {
            MyWorld.AddActorFeature<T>(this, tickGroup);
        }

        public void AddFeature(Feature feature, TickGroup tickGroup) {
            MyWorld.AddActorFeature(this, feature, tickGroup);
        }

        public void SetupAllFeatures() {
            MyWorld.SetupAllActorFeatures(this);
        }

        public void Destroy() {
            MyWorld.DestroyActor(this);
        }
    }
}