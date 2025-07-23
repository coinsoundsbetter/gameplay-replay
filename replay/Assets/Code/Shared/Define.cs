using System;
using System.Collections.Generic;
using FishNet.Serializing;
using System.Runtime.CompilerServices;

namespace KillCam
{
    /// <summary>
    /// 用于启动完整的功能
    /// 这样,我们可以只在需要的时机启动一些功能
    /// </summary>
    public class InitializeFeature : Feature
    {
    }

    /// <summary>
    /// 世界范围的功能继承它
    /// </summary>
    public class Feature
    {
        protected BattleWorld world { get; private set; }

        public void SetWorld(BattleWorld w)
        {
            world = w;
        }

        public virtual void OnCreate()
        {
        }

        public virtual void OnDestroy()
        {
        }

        protected T Get<T>() where T : Feature
        {
            return world.Get<T>();
        }

        protected uint GetTick() => world.Network.GetTick();
    }

    public interface INetworkSerialize
    {
        byte[] Serialize(Writer writer);
        void Deserialize(Reader reader);
        NetworkMsg GetMsgType();
    }

    public class RefStorageBase : IDisposable
    {
        public virtual void Dispose() { }
    }
    
    public class RefStorage<T> : RefStorageBase where T : unmanaged
    {
        public T Value;
        public ref T GetRef() => ref Unsafe.AsRef(in Value);

        public override void Dispose()
        {
            if (Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public class GameplayActor
    {
        public BattleWorld MyWorld { get; private set; }
        private readonly Dictionary<Type, RefStorageBase> unmanagedDataSet = new();
        private readonly Dictionary<Type, object> managedDataSet = new();
        private readonly Dictionary<string, Capability> capabilities = new();
        private readonly List<Capability> fixedCapabilities = new();
        private readonly List<Capability> frameCapabilities = new();

        public void SetupWorld(BattleWorld world)
        {
            MyWorld = world;
        }

        public void SetupData<T>(T instance) where T : unmanaged
        {
            var type = typeof(T);
            if (!unmanagedDataSet.ContainsKey(type))
            {
                unmanagedDataSet.Add(type, new RefStorage<T>() { Value = instance, });
            }
        }

        public void SetupDataManaged<T>(T instance) where T : class
        {
            var type = typeof(T);
            if (!managedDataSet.ContainsKey(type))
            {
                managedDataSet.Add(type, instance);
            }
        }

        public ref T GetDataReadWrite<T>() where T : unmanaged
        {
            var type = typeof(T);
            if (unmanagedDataSet.TryGetValue(type, out var storage))
            {
                return ref ((RefStorage<T>)storage).GetRef();
            }

            throw new KeyNotFoundException("no unmanaged data found");
        }

        public T GetDataReadOnly<T>() where T : unmanaged
        {
            var type = typeof(T);
            if (unmanagedDataSet.TryGetValue(type, out var storage))
            {
                return ((RefStorage<T>)storage).Value;
            }

            return default;
        }

        public T GetDataManaged<T>() where T : class
        {
            var type = typeof(T);
            if (managedDataSet.TryGetValue(type, out var obj))
            {
                return (T)obj;
            }

            return default;
        }

        public void SetupCapability<T>(TickGroup tickGroup) where T : Capability, new()
        {
            var typeName = typeof(T).Name;
            if (!capabilities.ContainsKey(typeName))
            {
                var capability = new T();
                capabilities.Add(typeName, capability);
                switch (tickGroup)
                {
                    case TickGroup.FixedStep:
                        fixedCapabilities.Add(capability);
                        break;
                    case TickGroup.FrameStep:
                        frameCapabilities.Add(capability);
                        break;
                }
                capability.Setup(this, tickGroup);
            }
        }

        public void OnOwnerDestroyed()
        {
            foreach (var c in capabilities.Values)
            {
                c.Destroy();
            }
            frameCapabilities.Clear();
            fixedCapabilities.Clear();

            foreach (var dataObj in managedDataSet.Values)
            {
                if (dataObj is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            managedDataSet.Clear();

            foreach (var dataUnmanaged in unmanagedDataSet.Values)
            {
                dataUnmanaged.Dispose();
            }
            unmanagedDataSet.Clear();
        }

        public void TickLogic(double deltaTime)
        {
            foreach (var c in fixedCapabilities)
            {
                if (!c.IsActive && c.OnShouldActivate())
                {
                    c.Activate();
                }
                else if (c.IsActive && c.OnShouldDeactivate())
                {
                    c.Deactivate();
                }
                if(c.IsActive)
                {
                    c.TickActive(deltaTime);
                }
            }
        }

        public void TickFrame(float deltaTime)
        {
            foreach (var c in frameCapabilities)
            {
                if (!c.IsActive && c.OnShouldActivate())
                {
                    c.Activate();
                }
                else if (c.IsActive && c.OnShouldDeactivate())
                {
                    c.Deactivate();
                }
                if(c.IsActive)
                {
                    c.TickActive(deltaTime);
                }
            }
        }
    }

    public abstract class Capability
    {
        protected GameplayActor Owner { get; private set; }
        protected BattleWorld World { get; private set; }
        public TickGroup MyTickGroup { get; private set; }
        public bool IsActive { get; private set; }
        public List<GameplayTag> Tags = new();
        protected double TickDelta { get; private set; }

        public void Setup(GameplayActor gameplayActor, TickGroup tickGroup)
        {
            Owner = gameplayActor;
            World = gameplayActor.MyWorld;
            MyTickGroup = tickGroup;
            IsActive = false;
            OnSetup();
        }

        public void Activate()
        {
            IsActive = true;
            OnActivate();
        }

        public void Deactivate()
        {
            IsActive = false;
            OnDeactivate();
        }

        public void Destroy()
        {
            if (IsActive)
            {
                OnDeactivate();
            }
            
            OnDestroy();
        }

        public void TickActive(double deltaTime)
        {
            TickDelta = deltaTime;
            OnTickActive();
        }

        protected virtual void OnSetup() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }
        protected virtual void OnTickActive() { }
        public virtual bool OnShouldActivate() { return true; }
        public virtual bool OnShouldDeactivate() { return false; }
    }

    public struct GameplayTag
    {
    }

    public enum TickGroup
    {
        FixedStep,
        FrameStep,
    }
}