using System.Collections.Generic;
using Unity.Collections;

namespace KillCam.Server {
    public class Server_SpawnHeroSystem : SystemBase {
        private readonly Dictionary<int, GameplayActor> roleActors = new();
        public IReadOnlyDictionary<int, GameplayActor> RoleActors => roleActors;
        
        /*protected override void OnCreate() {
            HeroNet.OnServerSpawn += OnRoleNetSpawn;
            HeroNet.OnServerDespawn += OnRoleNetDespawn;
        }

        protected override void OnDestroy() {
            HeroNet.OnServerSpawn -= OnRoleNetSpawn;
            HeroNet.OnServerDespawn -= OnRoleNetDespawn;
        }*/

        private void OnRoleNetSpawn(IServerHeroNet net) {
            var id = net.GetId();
            var character = CreateActor(ActorGroup.Player);
            character.AddData(new HeroIdentifier() {
                IsControlTarget = false,
                PlayerId = net.GetId(),
                PlayerName = $"Player:{net.GetId()}"
            });
            
            character.AddFeature<ServerHero_FireValidate>(TickGroup.Simulation);
            
            roleActors.Add(id, character);
        }

        private void OnRoleNetDespawn(IServerHeroNet net) {
            if (roleActors.Remove(net.GetId(), out var character)) {
                character.Destroy();
            }
        }
    }
}