using Unity.Entities;

namespace KillCam {
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class SingletonInstallSystem : SystemBase {
        
        protected override void OnCreate()
        {
            EntityManager.CreateSingleton<GameIdPool>();
            EntityManager.CreateSingleton<SendQueue>();
            EntityManager.CreateSingleton<RpcQueue>();
            EntityManager.CreateSingleton<NetChannels>();
            FishNetChannel.OnSpawned += OnSpawn;
            FishNetChannel.OnDespawn += OnDespawn;
        }

        protected override void OnDestroy()
        {
            FishNetChannel.OnSpawned -= OnSpawn;
            FishNetChannel.OnDespawn -= OnDespawn;
        }
        
        private void OnSpawn(FishNetChannel obj)
        {
            var netChannels = SystemAPI.ManagedAPI.GetSingleton<NetChannels>();
            netChannels.Channels.Add(obj.PlayerId.Value, obj);
            if (obj.IsOwner)
            {
                netChannels.LocalPlayerId = obj.PlayerId.Value;
            }
        }
        
        private void OnDespawn(FishNetChannel obj)
        {
            SystemAPI.ManagedAPI.GetSingleton<NetChannels>().Channels.Remove(obj.PlayerId.Value);
        }

        protected override void OnUpdate() { }
    }
}