namespace Gameplay.Core
{
    public class SystemBase : ISystem
    {
        protected World World { get; private set; }
        
        public virtual void OnCreate(World world)
        {
            World = world;
        }

        public virtual void OnDestroy() { }
        
        public virtual void Update(ref SystemState state) { }
        
        // ====================
        // Actor 管理
        // ====================
        protected Actor CreateActor()
            => World.ActorManager.CreateActor();

        protected void DestroyActor(Actor actor)
            => World.ActorManager.DestroyActor(actor);
        
        // ====================
        // 非托管数据访问（struct component）
        // ====================
        protected bool HasData<T>(Actor actor) where T : unmanaged
            => World.ActorManager.HasData<T>(actor);

        protected ref T GetDataRW<T>(Actor actor) where T : unmanaged
            => ref World.ActorManager.GetDataRW<T>(actor);

        protected ref readonly T GetDataRO<T>(Actor actor) where T : unmanaged
            => ref World.ActorManager.GetDataRO<T>(actor);

        protected void AddData<T>(Actor actor, T value) where T : unmanaged
            => World.ActorManager.AddData(actor, value);

        protected void RemoveData<T>(Actor actor) where T : unmanaged
            => World.ActorManager.RemoveData<T>(actor);
        
        // ====================
        // 托管数据访问（class component）
        // ====================
        protected bool HasDataManaged<T>(Actor actor) where T : class
            => World.ActorManager.HasDataManaged<T>(actor);

        protected T GetDataManaged<T>(Actor actor) where T : class
            => World.ActorManager.GetDataManaged<T>(actor);

        protected void AddDataManaged<T>(Actor actor, T value) where T : class
            => World.ActorManager.AddDataManaged(actor, value);

        protected void RemoveDataManaged<T>(Actor actor) where T : class
            => World.ActorManager.RemoveDataManaged<T>(actor);

        // ====================
        // 单例 Actor 支持
        // ====================
        protected Actor CreateSingleton<T>(T data) where T : unmanaged
            => World.ActorManager.CreateSingleton(data);

        protected ref T GetSingleton<T>() where T : unmanaged
            => ref World.ActorManager.GetSingleton<T>();

        protected bool HasSingleton<T>() where T : unmanaged
            => World.ActorManager.HasSingleton<T>();

        protected void DestroySingleton<T>() where T : unmanaged
            => World.ActorManager.DestroySingleton<T>();

        // （可选扩展：支持托管单例）
        /*protected Actor CreateSingletonManaged<T>(T data) where T : class
            => World.ActorManager.CreateSingletonManaged(data);

        protected T GetSingletonManaged<T>() where T : class
            => World.ActorManager.GetSingletonManaged<T>();

        protected bool HasSingletonManaged<T>() where T : class
            => World.ActorManager.HasSingletonManaged<T>();

        protected void DestroySingletonManaged<T>() where T : class
            => World.ActorManager.DestroySingletonManaged<T>();*/
    }
}
