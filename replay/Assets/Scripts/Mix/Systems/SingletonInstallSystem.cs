using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam {
    [UpdateBefore(typeof(InitializationSystemGroup))]
    public partial class SingletonInstallSystem : SystemBase {
        
        protected override void OnCreate()
        {
            CreateSingleton<LocalConnectState>("Singleton_LocalConnectState");
            CreateSingletonManaged<GameData>("Singleton_GameDict");
            CreateSingletonManaged<SendQueue>("Singleton_SendQueue");
            CreateSingletonManaged<RpcQueue>("Singleton_RpcQueue");
            CreateSingletonManaged<NetChannels>("Singleton_NetChannels");
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
            Debug.Log("Spawn !" );
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

        private void CreateSingletonManaged<T>(FixedString64Bytes name) where T : class, IComponentData, new() 
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.SetName(entity, name);
            EntityManager.AddComponentData(entity, new T());
        }
        
        private void CreateSingleton<T>(FixedString64Bytes name) where T : unmanaged, IComponentData
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.SetName(entity, name);
            EntityManager.AddComponentData(entity, new T());
        }
    }
}