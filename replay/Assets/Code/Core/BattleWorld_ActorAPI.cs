using System;
using System.Collections.Generic;
using Unity.Collections;

namespace KillCam {
    public partial class BattleWorld {
        private int actorIndex;
        
        public GameplayActor CreateActor(ActorGroup group = ActorGroup.Default) {
            var newActor = new GameplayActor();
            actorGroupMap.Add(newActor, group);
            groupActors[group].Add(newActor);
            actorFeatureSet.Add(newActor, new Dictionary<string, Feature>());
            actorDataManagedSet.Add(newActor, new Dictionary<Type, object>());
            actorDataUnmanagedSet.Add(newActor, new Dictionary<Type, RefStorageBase>());
            actorBufferUnmanagedSet.Add(newActor, new Dictionary<Type, RefStorageBase>());
            newActor.Setup(WorldId, actorIndex);
            actorIndex++;
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

            foreach (var data in actorBufferUnmanagedSet[actor].Values) {
                data.Dispose();
            }
            actorBufferUnmanagedSet.Remove(actor);
        }
        
        public void AddActorFeature<T>(GameplayActor actor, TickGroup tickGroup) where T : Feature, new() {
            var t = new T();
            actorFeatureSet[actor].Add(typeof(T).Name, t);
            tickGroups[tickGroup].Add(t);
            t.Create(actor);
        }

        public void AddActorFeature(GameplayActor actor, Feature feature, TickGroup tickGroup) {
            var type = feature.GetType();
            actorFeatureSet[actor].Add(type.Name, feature);
            tickGroups[tickGroup].Add(feature);
            feature.Create(actor);
        }

        public void SetupAllActorFeatures(GameplayActor actor) {
            var set = actorFeatureSet[actor];
            foreach (var feature in set.Values) {
                feature.Setup();
            }
        }

        public void AddActorData<T>(GameplayActor actor, T instance) where T : unmanaged {
            var type = typeof(T);
            actorDataUnmanagedSet[actor].Add(type, new RefStorage<T>() { Value = instance });
        }

        public void AddActorDataManaged<T>(GameplayActor actor, T instance) where T : class {
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

        public bool HasActorDataManaged<T>(GameplayActor actor) where T : class {
            var type = typeof(T);
            if (actorDataManagedSet[actor].ContainsKey(type)) {
                return true;
            }

            return false;
        }

        public bool TryGetActorDataManaged<T>(GameplayActor actor, out T value) where T : class {
            var type = typeof(T);
            if (actorDataManagedSet[actor].TryGetValue(type, out var res)) {
                value = (T)res;
                return true;
            }

            value = default;
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

        public bool HasActorData(GameplayActor actor, Type type) {
            var unmanagedSet = actorDataUnmanagedSet[actor];
            if (unmanagedSet.ContainsKey(type)) {
                return true;
            }

            var managedSet = actorDataManagedSet[actor];
            if (managedSet.ContainsKey(type)) {
                return true;
            }

            return false;
        }

        public void SetupActorBuffer<T>(GameplayActor actor) where T : unmanaged, IBufferElement {
            var type = typeof(T);
            actorBufferUnmanagedSet[actor].Add(type, new RefStorageBuffer<T>() {
                Value = new DynamicBuffer<T>(0, Allocator.Persistent),
            });
        }

        public ref DynamicBuffer<T> GetActorBuffer<T>(GameplayActor actor) where T : unmanaged, IBufferElement {
            var type = typeof(T);
            if (actorBufferUnmanagedSet[actor].TryGetValue(type, out var buffWrapper)) {
                return ref ((RefStorageBuffer<T>)buffWrapper).GetRef();
            }
            
            throw new KeyNotFoundException($"No dynamic buffer of type {type}");
        }
    }
}