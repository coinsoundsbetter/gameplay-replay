using System.Collections.Generic;

namespace KillCam.Client {
    public class Client_SpawnHeroSystem : SystemBase {
        private readonly IRoleSpawnProvider provider;
        private readonly Dictionary<int, GameplayActor> characters = new();
        public IReadOnlyDictionary<int, GameplayActor> Characters => characters;

        public Client_SpawnHeroSystem(IRoleSpawnProvider provider) {
            this.provider = provider;
        }

        /*protected override void OnCreate() {
            provider.OnRoleSpawn += OnRoleSpawn;
            provider.OnRoleDespawn += OnRoleDespawn;
        }

        protected override void OnDestroy() {
            provider.OnRoleSpawn -= OnRoleSpawn;
            provider.OnRoleDespawn -= OnRoleDespawn;
        }*/

        private void OnRoleSpawn(IClientHeroNet net) {
            var playerId = net.GetId();
            if (characters.ContainsKey(playerId)) {
                return;
            }

            var characterActor = CreateActor(ActorGroup.Player);
            // 设置数据
            characterActor.AddData(new HeroIdentifier() {
                PlayerId = playerId,
                IsControlTarget = net.IsClientOwned(),
                PlayerName = $"玩家{playerId}"
            });
            characterActor.SetDataManaged(new UnityHeroLink());
            characterActor.SetDataManaged(new ClientHeroNetLink() {
                NetClient = net,
            });
            
            // 设置逻辑
            characterActor.AddFeature<HeroInput>(TickGroup.Input);
            characterActor.AddFeature<ClientHero_FireIntend>(TickGroup.Simulation);
            characterActor.AddFeature<ClientHero_FirePredictBullet>(TickGroup.Simulation);
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