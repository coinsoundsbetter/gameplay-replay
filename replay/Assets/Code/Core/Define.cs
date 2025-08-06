using System;
using System.Collections.Generic;
using FishNet.Serializing;
using System.Runtime.CompilerServices;
using FishNet.Managing;
using UnityEngine;

namespace KillCam {
    /// <summary>
    /// 用于启动完整的功能
    /// 这样,我们可以只在需要的时机启动一些功能
    /// </summary>
    public abstract class Boostrap {
        protected BattleWorld world { get; private set; }
        
        public void Start(BattleWorld myWorld) {
            world = myWorld;
            OnBeforeInitialize();
        }

        public void End() {
            OnAfterCleanup();
        }
        
        protected abstract void OnBeforeInitialize();
        protected abstract void OnAfterCleanup();
    }

    /// <summary>
    /// 客户端初始化
    /// </summary>
    public abstract class ClientBoostrapBase : Boostrap {
        public ClientBoostrapBase(NetworkManager manager) {
        }
    }

    /// <summary>
    /// 回放初始化
    /// </summary>
    public abstract class ReplayBoostrapBase : Boostrap {
    }

    /// <summary>
    /// 服务器初始化
    /// </summary>
    public abstract class ServerBoostrapBase : Boostrap {
        public ServerBoostrapBase(NetworkManager manager) {
        }
    }

    [System.Flags]
    public enum WorldFlag {
        Default = 0,
        Client = 1 << 0,
        Server = 1 << 1,
        Replay = 1 << 2,
    }

    public interface INetworkMsg {
        byte[] Serialize(Writer writer);
        void Deserialize(Reader reader);
        ushort GetMsgType();
    }

    public interface INetworkClient {
        void Send<T>(T message) where T : INetworkMsg;
        uint GetTick();
    }

    public interface INetworkServer {
        void Rpc<T>(T message) where T : INetworkMsg;
        void TargetRpc<T>(int id, T message) where T : INetworkMsg;
        uint GetTick();
    }

    public class RefStorageBase : IDisposable {
        public virtual void Dispose() {
        }
    }

    public class RefStorage<T> : RefStorageBase where T : unmanaged {
        public T Value;
        public ref T GetRef() => ref Unsafe.AsRef(in Value);

        public override void Dispose() {
            if (Value is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }

    public class GameplayActor {
        public BattleWorld MyWorld { get; private set; }

        public void SetupWorld(BattleWorld world) {
            MyWorld = world;
        }

        public void SetupData<T>(T instance) where T : unmanaged {
            MyWorld.SetupData(this, instance);
        }

        public void SetupDataManaged<T>(T instance) where T : class {
            MyWorld.SetupDataManaged(this, instance);
        }

        public ref T GetDataReadWrite<T>() where T : unmanaged {
            return ref MyWorld.GetDataRW<T>(this);
        }

        public T GetDataReadOnly<T>() where T : unmanaged {
            return MyWorld.GetDataRO<T>(this);
        }

        public T GetDataManaged<T>() where T : class {
            return MyWorld.GetDataManaged<T>(this);
        }

        public void SetupCapability<T>(TickGroup tickGroup) where T : Feature, new() {
            MyWorld.SetupFeature<T>(this, tickGroup);
        }

        public void SetupCapability(Feature feature, TickGroup tickGroup) {
            MyWorld.SetupFeature(this, feature, tickGroup);
        }

        public void Destroy() {
            MyWorld.DestroyActor(this);
        }
    }

    public interface ICapability {
        
    }

    public abstract class Feature {
        protected GameplayActor Owner { get; private set; }
        protected BattleWorld World { get; private set; }
        public bool IsActive { get; private set; }
        public List<GameplayTag> Tags = new();
        protected double TickDelta { get; private set; }

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

        protected virtual void OnSetup() {
        }

        protected virtual void OnDestroy() {
        }

        protected virtual void OnActivate() {
        }

        protected virtual void OnDeactivate() {
        }

        protected virtual void OnTickActive() {
        }

        public virtual bool OnShouldActivate() {
            return true;
        }

        public virtual bool OnShouldDeactivate() {
            return false;
        }
    }

    public struct GameplayTag {
    }

    public enum TickGroup {
        InitializeLogic,
        Input,
        PlayerLogic,
        
        InitializeFrame,
        PlayerFrame,
        CameraFrame,
    }

    public enum ActorGroup {
        Default,
        Player,
        World,
    }

    public class LayerDefine {
        public static readonly int defaultLayer = LayerMask.NameToLayer("Default");
        public static readonly int defaultLayerMask = 1 << defaultLayer;
        public static readonly int characterLayer = LayerMask.NameToLayer("Character");
        public static readonly int characterLayerMask = 1 << characterLayer;
        public static readonly int replayCharacterLayer = LayerMask.NameToLayer("ReplayCharacter");
        public static readonly int replayCharacterLayerMask = 1 << replayCharacterLayer;
    }
}