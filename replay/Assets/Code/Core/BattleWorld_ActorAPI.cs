using System;
using System.Collections.Generic;

namespace KillCam {
    public partial class BattleWorld {
        
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

        public bool HasData<T>() where T : unmanaged {
            return HasActorData<T>(worldActor);
        }

        public bool HasActorData<T>(GameplayActor actor) where T : unmanaged {
            var type = typeof(T);
            if (actorDataUnmanagedSet[actor].ContainsKey(type)) {
                return true;
            }

            return false;
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
    }
}