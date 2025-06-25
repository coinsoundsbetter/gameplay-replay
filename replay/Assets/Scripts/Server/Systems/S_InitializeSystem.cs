using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    public partial class S_InitializeSystem : SystemBase
    {
        protected override void OnStartRunning() {
            CreateSingletonManaged<GameData>("Singleton_GameData");
            CreateSingletonManaged<NetRpc>("Singleton_RpcQueue");
            CreateSingletonManaged<NetChannels>("Singleton_NetChannels");
            FishNetChannel.OnSpawned += OnSpawn;
            FishNetChannel.OnDespawn += OnDespawn;
        }

        protected override void OnStopRunning() {
            FishNetChannel.OnSpawned -= OnSpawn;
            FishNetChannel.OnDespawn -= OnDespawn;
        }

        protected override void OnUpdate() { }
        
        private void OnSpawn(FishNetChannel obj)
        {
            Debug.Log("Spawn " + obj);
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