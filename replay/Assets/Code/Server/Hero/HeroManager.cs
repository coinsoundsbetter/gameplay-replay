using System.Collections.Generic;

namespace KillCam.Server {
    public class HeroManager : Feature {
        private readonly Dictionary<int, Hero> roleActors = new();
        public IReadOnlyDictionary<int, Hero> RoleActors => roleActors;

        public override void OnCreate() {
            HeroNet.OnServerSpawn += OnRoleNetSpawn;
            HeroNet.OnServerDespawn += OnRoleNetDespawn;
        }

        public override void OnDestroy() {
            HeroNet.OnServerSpawn -= OnRoleNetSpawn;
            HeroNet.OnServerDespawn -= OnRoleNetDespawn;
        }

        private void OnRoleNetSpawn(IServerHeroNet net) {
            var id = net.GetId();
            var character = new Hero() {
                Net = net,
            };
            character.SetupWorld(world);
            character.SetupData(new HeroIdentifier() {
                IsControlTarget = false,
                PlayerId = net.GetId(),
                PlayerName = $"Player:{net.GetId()}"
            });
            character.SetupData(new HeroInputData());
            character.SetupData(new HeroMoveData());
            character.SetupCapability<CharacterMovement>(TickGroup.FixedStep);
            //character.SetupCapability<CharacterView>(TickGroup.FrameStep);
            Get<ActorManager>().RegisterActor(character);
            roleActors.Add(id, character);
        }

        private void OnRoleNetDespawn(IServerHeroNet net) {
            if (roleActors.Remove(net.GetId(), out var character)) {
                character.OnOwnerDestroyed();
                Get<ActorManager>().UnregisterActor(character);
            }
        }
    }
}