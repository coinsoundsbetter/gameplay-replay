using System.Collections.Generic;
using UnityEngine;

namespace KillCam.Client {
    public class HeroManager : Feature {
        private readonly IRoleSpawnProvider provider;
        private readonly Dictionary<int, Hero> characters = new();
        public IReadOnlyDictionary<int, Hero> Characters => characters;

        public HeroManager(IRoleSpawnProvider provider) {
            this.provider = provider;
        }

        public override void OnCreate() {
            provider.OnRoleSpawn += OnRoleSpawn;
            provider.OnRoleDespawn += OnRoleDespawn;
        }

        public override void OnDestroy() {
            provider.OnRoleSpawn -= OnRoleSpawn;
            provider.OnRoleDespawn -= OnRoleDespawn;
        }

        private void OnRoleSpawn(IClientHeroNet net) {
            var playerId = net.GetId();
            if (characters.ContainsKey(playerId)) {
                return;
            }

            var characterActor = new Hero() {
                Net = net,
            };
            characterActor.SetupWorld(world);
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
            
            // 设置逻辑
            characterActor.SetupCapability<HeroInput>(TickGroup.Input);
            characterActor.SetupCapability<HeroMovement>(TickGroup.FixedStep);
            characterActor.SetupCapability<HeroVisualSkin>(TickGroup.FrameStep);
            characterActor.SetupCapability<HeroVisualAnim>(TickGroup.FrameStep);
            characterActor.SetupCapability<HeroVisualMove>(TickGroup.FrameStep);
            characterActor.SetupCapability<HeroVisualCamera>(TickGroup.FrameStep);
         //   characterActor.SetupCapability<herovisua>();
            Get<ActorManager>().RegisterActor(characterActor);
            characters.Add(playerId, characterActor);
        }

        private void OnRoleDespawn(IHeroNet net) {
            var playerId = net.GetId();
            if (characters.Remove(playerId, out var character)) {
                character.OnOwnerDestroyed();
                Get<ActorManager>().UnregisterActor(character);
            }
        }
    }
}