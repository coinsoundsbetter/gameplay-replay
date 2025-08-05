using System;
using System.Collections.Generic;
using Unity.Collections;

namespace KillCam {
    public class BattleWorld {
        public WorldFlag Flags { get; private set; }
        public FixedString32Bytes Name { get; private set; }
        public float FrameTickDelta { get; private set; }
        public double LogicTickDelta { get; private set; }

        private Boostrap boostrap;
        private GameplayActor worldActor;
        private readonly Dictionary<TickGroup, List<Capability>> tickGroups = new();
        private readonly Dictionary<ActorGroup, List<GameplayActor>> actorGroups = new();
        private readonly Dictionary<GameplayActor, Dictionary<string, Capability>> actorCapabilitySet = new();
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

        private readonly ActorGroup[] actorGroupTypes = {
            ActorGroup.Default,
            ActorGroup.World,
            ActorGroup.Player,
        };

        public void Init(WorldFlag flag, Boostrap bs, FixedString32Bytes worldName) {
            Flags = flag;
            Name = worldName;
            boostrap = bs;
            foreach (var actorGroupType in actorGroupTypes) {
                actorGroups.Add(actorGroupType, new List<GameplayActor>());
            }
            foreach (var logicTickGroupType in logicTickOrders) {
                tickGroups.Add(logicTickGroupType, new List<Capability>());
            }
            foreach (var frameTickGroupType in frameTickOrders) {
                tickGroups.Add(frameTickGroupType, new List<Capability>());
            }

            worldActor = CreateActor(ActorGroup.World);
            boostrap.Start(this);
        }
        
        public void Cleanup() {
            if (actorGroups.Remove(ActorGroup.Default, out var defaultList)) {
                foreach (var actor in defaultList) {
                    actor.Destroy();
                }
                defaultList.Clear();
            }
            
            if (actorGroups.Remove(ActorGroup.Player, out var playerList)) {
                foreach (var actor in playerList) {
                    actor.Destroy();
                }
                playerList.Clear();
            }

            if (actorGroups.Remove(ActorGroup.World, out var worldList)) {
                foreach (var actor in worldList) {
                    actor.Destroy();
                }
                worldList.Clear();
            }
            
            actorGroups.Clear();
            boostrap.End();
        }

        public GameplayActor CreateActor(ActorGroup group = ActorGroup.Default) {
            var newActor = new GameplayActor();
            actorGroups[group].Add(newActor);
            actorCapabilitySet.Add(newActor, new Dictionary<string, Capability>());
            actorDataManagedSet.Add(newActor, new Dictionary<Type, object>());
            actorDataUnmanagedSet.Add(newActor, new Dictionary<Type, RefStorageBase>());
            newActor.SetupWorld(this);
            return newActor;
        }

        public void DestroyActor(GameplayActor actor) {
            if (actorGroups[ActorGroup.Default].Remove(actor)) {
                CleanupActorBody();
                return;
            }
            
            if (actorGroups[ActorGroup.Player].Remove(actor)) {
                CleanupActorBody();
                return;
            }
            
            if (actorGroups[ActorGroup.World].Remove(actor)) {
                CleanupActorBody();
                return;
            }

            void CleanupActorBody() {
                foreach (var c in actorCapabilitySet[actor].Values) {
                    c.Destroy();
                }
                actorCapabilitySet.Remove(actor);

                foreach (var obj in actorDataManagedSet[actor].Values) {
                    if (obj is IDisposable disposable) {
                        disposable.Dispose();
                    }
                }
                actorCapabilitySet.Remove(actor);

                foreach (var data in actorDataUnmanagedSet[actor].Values) {
                    data.Dispose();
                }
                actorDataUnmanagedSet.Remove(actor);
            }
        }

        public void FrameUpdate(float delta) {
            FrameTickDelta = delta;
            Tick(TickGroup.InitializeFrame, delta);
            Tick(TickGroup.PlayerFrame, delta);
            Tick(TickGroup.CameraFrame, delta);
        }

        public void LogicUpdate(double delta) {
            LogicTickDelta = delta;
            Tick(TickGroup.InitializeLogic, delta);
            Tick(TickGroup.Input, delta);
            Tick(TickGroup.PlayerLogic, delta);
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

        public void SetupCapability<T>(GameplayActor actor, TickGroup tickGroup) where T : Capability, new() {
            var t = new T();
            actorCapabilitySet[actor].Add(typeof(T).Name, t);
            tickGroups[tickGroup].Add(t);
            t.Setup(actor);
        }

        public void SetupCapability(GameplayActor actor, Capability capability, TickGroup tickGroup) {
            var type = capability.GetType();
            actorCapabilitySet[actor].Add(type.Name, capability);
            tickGroups[tickGroup].Add(capability);
            capability.Setup(actor);
        }

        public void SetupData<T>(GameplayActor actor, T instance) where T : unmanaged {
            var type = typeof(T);
            actorDataUnmanagedSet[actor].Add(type, new RefStorage<T>() { Value = instance });
        }

        public void SetupDataManaged<T>(GameplayActor actor, T instance) where T : class {
            var type = typeof(T);
            actorDataManagedSet[actor].Add(type, instance);
        }
        
        public ref T GetDataReadWrite<T>(GameplayActor actor) where T : unmanaged {
            var type = typeof(T);
            if (actorDataUnmanagedSet[actor].TryGetValue(type, out var storage)) {
                return ref ((RefStorage<T>)storage).GetRef();
            }

            throw new KeyNotFoundException("no unmanaged data found");
        }

        public T GetDataReadOnly<T>(GameplayActor actor) where T : unmanaged {
            var type = typeof(T);
            if (actorDataUnmanagedSet[actor].TryGetValue(type, out var storage)) {
                return ((RefStorage<T>)storage).Value;
            }

            return default;
        }

        public T GetDataManaged<T>(GameplayActor actor) where T : class {
            var type = typeof(T);
            if (actorDataManagedSet[actor].TryGetValue(type, out var obj)) {
                return (T)obj;
            }

            return default;
        }

        public T GetFunction<T>() where T : Capability {
            var set = actorCapabilitySet[worldActor];
            if (set.TryGetValue(typeof(T).Name, out var c)) {
                return (T)c;
            }

            return default;
        }

        public ref readonly T GetWorldDataRO<T>() where T : unmanaged {
            var set = actorDataUnmanagedSet[worldActor];
            if (set.TryGetValue(typeof(T), out var v)) {
                return ref ((RefStorage<T>)v).Value;
            }

            throw new KeyNotFoundException($"No data of type {typeof(T)}");
        }

        public ref T GetWorldDataRW<T>() where T : unmanaged {
            var set = actorDataManagedSet[worldActor];
            if (set.TryGetValue(typeof(T), out var v)) {
                return ref ((RefStorage<T>)v).Value;
            }
            
            throw new KeyNotFoundException($"No data of type {typeof(T)}");
        }

        public bool HasFlag(WorldFlag check) {
            if ((Flags & check) != 0) {
                return true;
            }

            return false;
        }
    }
}