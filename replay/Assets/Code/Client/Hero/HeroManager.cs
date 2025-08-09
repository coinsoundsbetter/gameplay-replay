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

        protected override void OnSetup() {
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
            characterActor.SetupDataManaged(new UnityHeroLink());
            characterActor.SetupDataManaged(new ClientHeroNetLink() {
                NetClient = net,
            });
            
            // 设置逻辑
            characterActor.SetupFeature<HeroInput>(TickGroup.Input);
            characterActor.SetupFeature<HeroMovement>(TickGroup.Simulation);
            characterActor.SetupFeature<HeroFire>(TickGroup.Simulation);
            characterActor.SetupFeature<HeroVisualSkin>(TickGroup.Visual);
            characterActor.SetupFeature<HeroVisualAnim>(TickGroup.Visual);
            characterActor.SetupFeature<HeroVisualMove>(TickGroup.Visual);
            characterActor.SetupFeature<HeroVisualCamera>(TickGroup.Visual);
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