using System;
using System.Collections.Generic;

namespace KillCam
{
    public abstract class SystemBase : ISystem
    {
        private GameplayActor _owner;
        private BattleWorld _world;

        public GameplayActor Owner => _owner;
        public BattleWorld World => _world;
        public bool IsActive { get; private set; }
        protected double TickDeltaDouble { get; private set; }
        protected float TickDelta => (float)TickDeltaDouble;

        //====================
        // 生命周期管理
        //====================
        public void Create(GameplayActor actor)
        {
            _owner = actor;
            _world = actor.MyWorld;
            IsActive = false;
            OnCreate(_world, _owner);
        }

        public void Setup() => OnSetup();

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                OnActivate();
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                OnDeactivate();
            }
        }

        public void Create(BattleWorld world) {
            
        }

        public void Destroy()
        {
            if (IsActive) Deactivate();
            OnDestroy();
        }

        public void Tick(double deltaTime)
        {
            TickDeltaDouble = deltaTime;
            OnTick();
        }

        //====================
        // 生命周期回调（子类重写）
        //====================
        protected virtual void OnCreate(BattleWorld world, GameplayActor owner) { }
        protected virtual void OnSetup() { }
        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnTick() { }
        public virtual bool OnShouldActivate() => true;

        //====================
        // Actor 级数据访问
        //====================
        protected void CreateData<T>() where T : unmanaged
            => _world.AddData(new T());

        protected bool HasData<T>() where T : unmanaged
            => _world.HasActorData<T>(_owner);

        protected bool HasDataManaged<T>() where T : class
            => _world.HasActorDataManaged<T>(_owner);

        protected bool TryGetDataManaged<T>(out T value) where T : class
            => _world.TryGetActorDataManaged(_owner, out value);

        protected ref T GetDataRef<T>() where T : unmanaged
            => ref _world.GetActorDataRW<T>(_owner);

        protected bool TryGetData<T>(out T value) where T : unmanaged
        {
            if (HasData<T>())
            {
                value = GetDataRO<T>();
                return true;
            }
            value = default;
            return false;
        }

        protected ref readonly T GetDataRO<T>() where T : unmanaged
            => ref _world.GetActorDataRO<T>(_owner);

        protected void AddBuffer<T>() where T : unmanaged, IBufferElement
            => _world.SetupActorBuffer<T>(_owner);

        protected ref DynamicBuffer<T> GetBuffer<T>() where T : unmanaged, IBufferElement
            => ref _world.GetActorBuffer<T>(_owner);

        protected T GetDataManaged<T>() where T : class
            => _world.GetActorDataManaged<T>(_owner);

        protected GameplayActor CreateActor(ActorGroup actorGroup = ActorGroup.Default)
            => _world.CreateActor(actorGroup);

        protected void DestroyActor(GameplayActor actor)
            => _world.DestroyActor(actor);

        //====================
        // World 级数据访问
        //====================
        protected bool HasWorldData<T>() where T : unmanaged
            => _world.HasData<T>();

        protected ref T GetWorldDataRef<T>() where T : unmanaged
            => ref _world.GetDataRW<T>();

        protected ref readonly T GetWorldData<T>() where T : unmanaged
            => ref _world.GetDataRO<T>();

        protected T GetSingletonManaged<T>() where T : class
            => _world.GetDataManaged<T>();

        protected T GetSingletonFeature<T>() where T : SystemBase
            => _world.GetFeature<T>();

        protected ref DynamicBuffer<T> GetWorldBuffer<T>() where T : unmanaged, IBufferElement
            => ref _world.GetBuffer<T>();

        protected bool HasWorldFlag(WorldFlag flag)
            => _world.HasFlag(flag);

        //====================
        // 网络工具
        //====================
        protected void Send<T>(T msg) where T : INetworkMsg
            => _world.NetworkContext.SendToServer(msg);

        protected void BroadcastAll<T>(T msg) where T : INetworkMsg
            => _world.NetworkContext.SendToAllClients(msg);

        protected long GetRTT() => _world.NetworkContext.GetRTT();
        protected long GetHalfRTT() => _world.NetworkContext.GetRTT() / 2;
        protected double GetNetTime() => _world.NetworkContext.GetNowTime();
        protected uint GetTick() => _world.NetworkContext.GetTick();

        protected IEnumerable<GameplayActor> GetActors(ActorGroup group)
            => _world.GetActors(group);
    }
}
