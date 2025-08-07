using System;
using System.Collections.Generic;
using Unity.Collections;

namespace KillCam {
    public class BattleWorld {
        public WorldFlag Flags { get; private set; }
        public FixedString32Bytes Name { get; private set; }
        public float FrameTickDelta { get; private set; }
        public double LogicTickDelta { get; private set; }

        private GameEntry boostrap;
        private GameplayActor worldActor;
        private readonly Dictionary<TickGroup, List<Feature>> tickGroups = new();
        private readonly Dictionary<ActorGroup, List<GameplayActor>> groupActors = new();
        private readonly Dictionary<GameplayActor, ActorGroup> actorGroupMap = new();
        private readonly Dictionary<GameplayActor, Dictionary<string, Feature>> actorFeatureSet = new();
        private readonly Dictionary<GameplayActor, Dictionary<Type, object>> actorDataManagedSet = new();
        private readonly Dictionary<GameplayActor, Dictionary<Type, RefStorageBase>> actorDataUnmanagedSet = new();

        private readonly TickGroup[] logicTickOrders = {
            TickGroup.InitializeLogic,
            TickGroup.Input,
            TickGroup.PlayerLogic,
        };

        private readonly TickGroup[] frameTickOrders = {
            TickGroup.InitializeFrame,
            TickGroup.PlayerFrame,
            TickGroup.CameraFrame,
        };

        private readonly ActorGroup[] actorRemoveOrders = {
            ActorGroup.Default,
            ActorGroup.Player,
            ActorGroup.World,
        };

        public void Init(WorldFlag flag, GameEntry bs, FixedString32Bytes worldName) {
            Flags = flag;
            Name = worldName;
            boostrap = bs;
            foreach (var actorGroupType in actorRemoveOrders) {
                groupActors.Add(actorGroupType, new List<GameplayActor>());
            }
            foreach (var logicTickGroupType in logicTickOrders) {
                tickGroups.Add(logicTickGroupType, new List<Feature>());
            }
            foreach (var frameTickGroupType in frameTickOrders) {
                tickGroups.Add(frameTickGroupType, new List<Feature>());
            }

            worldActor = CreateActor(ActorGroup.World);
            boostrap.Start(this);
        }
        
        public void Destroy() {
            foreach (var actorGroup in actorRemoveOrders) {
                DestroyGroupAllActors(actorGroup);    
            }
            boostrap.End();
        }

        private void DestroyGroupAllActors(ActorGroup actorGroup) {
            if (groupActors.TryGetValue(actorGroup, out var actorList)) {
                int len = actorList.Count;
                for (int i = len - 1; i >= 0; i--) {
                    DestroyActor(actorList[i]);
                }
                
                groupActors.Remove(actorGroup);
            }
        }
        
        public GameplayActor CreateActor(ActorGroup group = ActorGroup.Default) {
            var newActor = new GameplayActor();
            actorGroupMap.Add(newActor, group);
            groupActors[group].Add(newActor);
            actorFeatureSet.Add(newActor, new Dictionary<string, Feature>());
            actorDataManagedSet.Add(newActor, new Dictionary<Type, object>());
            actorDataUnmanagedSet.Add(newActor, new Dictionary<Type, RefStorageBase>());
            newActor.SetupWorld(this);
            return newActor;
        }

        public void DestroyActor(GameplayActor actor) {
            actorGroupMap.Remove(actor, out var group);
            groupActors[group].Remove(actor);
            
            foreach (var c in actorFeatureSet[actor].Values) {
                c.Destroy();
            }
            actorFeatureSet.Remove(actor);

            foreach (var obj in actorDataManagedSet[actor].Values) {
                if (obj is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
            actorFeatureSet.Remove(actor);

            foreach (var data in actorDataUnmanagedSet[actor].Values) {
                data.Dispose();
            }
            actorDataUnmanagedSet.Remove(actor);
        }

        public void FrameUpdate(float delta) {
            FrameTickDelta = delta;
            foreach (var t in frameTickOrders) {
                Tick(t, delta);
            }
        }

        public void LogicUpdate(double delta) {
            LogicTickDelta = delta;
            foreach (var t in logicTickOrders) {
                Tick(t, delta);
            }
        }

        private void Tick(TickGroup group, double deltaTime) {
            foreach (var c in tickGroups[group]) {
                if (!c.IsActive && c.OnShouldActivate()) {
                    c.Activate();
                } else if (c.IsActive && c.OnShouldDeactivate()) {
                    c.Deactivate();
                }

                if (c.IsActive) {
                    c.TickActive(deltaTime);
                }
            }
        }

        public void SetupActorFeature<T>(GameplayActor actor, TickGroup tickGroup) where T : Feature, new() {
            var t = new T();
            actorFeatureSet[actor].Add(typeof(T).Name, t);
            tickGroups[tickGroup].Add(t);
            t.Setup(actor);
        }

        public void SetupActorFeature(GameplayActor actor, Feature feature, TickGroup tickGroup) {
            var type = feature.GetType();
            actorFeatureSet[actor].Add(type.Name, feature);
            tickGroups[tickGroup].Add(feature);
            feature.Setup(actor);
        }

        public void SetupActorData<T>(GameplayActor actor, T instance) where T : unmanaged {
            var type = typeof(T);
            actorDataUnmanagedSet[actor].Add(type, new RefStorage<T>() { Value = instance });
        }

        public void SetupActorDataManaged<T>(GameplayActor actor, T instance) where T : class {
            var type = typeof(T);
            actorDataManagedSet[actor].Add(type, instance);
        }
        
        public ref T GetActorDataRW<T>(GameplayActor actor) where T : unmanaged {
            var type = typeof(T);
            if (actorDataUnmanagedSet[actor].TryGetValue(type, out var storage)) {
                return ref ((RefStorage<T>)storage).GetRef();
            }

            throw new KeyNotFoundException("no unmanaged data found");
        }

        public ref readonly T GetActorDataRO<T>(GameplayActor actor) where T : unmanaged {
            var set = actorDataUnmanagedSet[actor];
            if (set.TryGetValue(typeof(T), out var v)) {
                return ref ((RefStorage<T>)v).Value;
            }

            throw new KeyNotFoundException($"No data of type {typeof(T)}");
        }

        public T GetActorDataManaged<T>(GameplayActor actor) where T : class {
            var type = typeof(T);
            if (actorDataManagedSet[actor].TryGetValue(type, out var obj)) {
                return (T)obj;
            }

            return default;
        }

        #region World
        
        public T GetFeature<T>() where T : Feature {
            var set = actorFeatureSet[worldActor];
            if (set.TryGetValue(typeof(T).Name, out var c)) {
                return (T)c;
            }

            return default;
        }

        public void SetupFeature<T>(TickGroup tickGroup) where T : Feature, new() 
            => SetupActorFeature<T>(worldActor, tickGroup);

        public void SetupFeature(Feature feature, TickGroup tickGroup) 
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
        
        public bool HasFlag(WorldFlag check) {
            if ((Flags & check) != 0) {
                return true;
            }

            return false;
        }

        #endregion
    }
}