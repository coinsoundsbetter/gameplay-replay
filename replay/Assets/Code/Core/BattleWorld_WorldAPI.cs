namespace KillCam {
    public partial class BattleWorld {
        
        public T GetFeature<T>() where T : SystemBase {
            var set = actorFeatureSet[worldActor];
            if (set.TryGetValue(typeof(T).Name, out var c)) {
                return (T)c;
            }

            return default;
        }

        public void AddFeature<T>(TickGroup tickGroup = TickGroup.Simulation) where T : SystemBase, new() 
            => AddActorFeature<T>(worldActor, tickGroup);

        public void AddFeature(SystemBase systemBase, TickGroup tickGroup = TickGroup.Simulation) 
            => AddActorFeature(worldActor, systemBase, tickGroup);

        public void SetupAllFeatures()
            => SetupAllActorFeatures(worldActor);
        
        public void AddData<T>(T instance) where T : unmanaged
            => AddActorData(worldActor, instance);
        
        public void SetupDataManaged<T>(T instance) where T : class 
            => AddActorDataManaged(worldActor, instance);

        public ref readonly T GetDataRO<T>() where T : unmanaged 
            => ref GetActorDataRO<T>(worldActor);

        public ref T GetDataRW<T>() where T : unmanaged 
            => ref GetActorDataRW<T>(worldActor);

        public T GetDataManaged<T>() where T : class {
            return GetActorDataManaged<T>(worldActor);
        }

        public void SetupBuffer<T>() where T : unmanaged, IBufferElement{
            SetupActorBuffer<T>(worldActor);
        }

        public ref DynamicBuffer<T> GetBuffer<T>() where T : unmanaged, IBufferElement {
            return ref GetActorBuffer<T>(worldActor);
        }
        
        public bool HasFlag(WorldFlag check) {
            if ((Flags & check) != 0) {
                return true;
            }

            return false;
        }
    }
}