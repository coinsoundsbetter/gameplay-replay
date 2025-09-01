using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Server {

    public struct NetHeroIndex : IBufferElement {
        public int id;
        public Actor actor;
    }
    
    [Order(SystemOrder.Last)]
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    [SystemTag(SystemFlag.Server)]
    public class ServerHeroSpawnSystem : ISystem {
        
        public void OnCreate(ref SystemState state) {
            state.ActorManager.CreateSingletonBuffer<NetHeroIndex>();
        }

        public void Update(ref SystemState state) {
            ref var buffer = ref state.ActorManager.GetSingletonBuffer<HeroSpawner>();
            for (int i = 0; i < buffer.Length; i++) {
                var elem = buffer[i];
                SpawnHero(ref state, elem);
            }
            buffer.Clear();
        }

        private void SpawnHero(ref SystemState state, HeroSpawner spawner) {
            ref var allHero = ref state.ActorManager.GetSingletonBuffer<NetHeroIndex>();
            allHero.Add(new NetHeroIndex() {
                id = spawner.Id,
                actor = state.ActorManager.CreateActor(),
            });
            
            state.ActorManager.GetSingletonManaged<NetworkServer>().SendToAll(new Protocol.CreatePlayer() {
                PlayerId = spawner.Id,
                Position = Vector3.zero,
            });
        }
    }
}