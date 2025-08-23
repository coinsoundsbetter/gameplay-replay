using System.Collections.Generic;
using UnityEngine;

namespace KillCam.Client {
    public class HeroManager : Feature {
        private readonly IRoleSpawnProvider provider;
        private readonly Dictionary<int, GameplayActor> characters = new();
        public IReadOnlyDictionary<int, GameplayActor> Characters => characters;

        public HeroManager(IRoleSpawnProvider provider) {
            this.provider = provider;
        }

        protected override void OnCreate() {
            provider.OnRoleSpawn += OnRoleSpawn;
            provider.OnRoleDespawn += OnRoleDespawn;
        }

        protected override void OnDestroy() {
            provider.OnRoleSpawn -= OnRoleSpawn;
            provider.OnRoleDespawn -= OnRoleDespawn;
        }

        private void OnRoleSpawn(IClientHeroNet net) {
            var playerId = net.GetId();
            if (characters.ContainsKey(playerId)) {
                return;
            }

            var characterActor = CreateActor(ActorGroup.Player);
            // 设置数据
            characterActor.SetupData(new HeroIdentifier() {
                PlayerId = playerId,
                IsControlTarget = net.IsClientOwned(),
                PlayerName = $"玩家{playerId}"
            });
            characterActor.SetupData(new HeroInputData());
            characterActor.SetupData(new HeroMoveData() { Rot = Quaternion.identity, });
            characterActor.SetupData(new HeroSkinData());
            characterActor.SetupData(new HeroAnimData());
            characterActor.SetupData(new HeroFireData());
            characterActor.SetupDataManaged(new UnityHeroLink());
            characterActor.SetupDataManaged(new ClientHeroNetLink() {
                NetClient = net,
            });
            
            // 设置逻辑
            characterActor.CreateFeature<HeroInput>(TickGroup.Input);
            characterActor.CreateFeature<HeroMovement>(TickGroup.Simulation);
            characterActor.CreateFeature<HeroFire>(TickGroup.Simulation);
            characterActor.CreateFeature<HeroVisualSkin>(TickGroup.Visual);
            characterActor.CreateFeature<HeroVisualAnim>(TickGroup.Visual);
            characterActor.CreateFeature<HeroVisualMove>(TickGroup.Visual);
            characterActor.CreateFeature<HeroVisualCamera>(TickGroup.Visual);
            characterActor.SetupAllFeatures();
            characters.Add(playerId, characterActor);
        }

        private void OnRoleDespawn(IHeroNet net) {
            var playerId = net.GetId();
            if (characters.Remove(playerId, out var character)) {
                character.Destroy();
            }
        }
    }
}