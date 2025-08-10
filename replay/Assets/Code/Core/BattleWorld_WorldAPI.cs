namespace KillCam {
    public partial class BattleWorld {
        
        public T GetFeature<T>() where T : Feature {
            var set = actorFeatureSet[worldActor];
            if (set.TryGetValue(typeof(T).Name, out var c)) {
                return (T)c;
            }

            return default;
        }

        public void SetupFeature<T>(TickGroup tickGroup = TickGroup.Simulation) where T : Feature, new() 
            => SetupActorFeature<T>(worldActor, tickGroup);

        public void SetupFeature(Feature feature, TickGroup tickGroup = TickGroup.Simulation) 
            => SetupActorFeature(worldActor, feature, tickGroup);
        
        public void SetupData<T>(T instance) where T : unmanaged
            => SetupActorData(worldActor, instance);
        

        public void SetupDataManaged<T>(T instance) where T : class 
            => SetupActorDataManaged(worldActor, instance);

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