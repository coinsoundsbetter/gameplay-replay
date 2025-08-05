using System.Collections.Generic;

namespace KillCam.Server {
    public class HeroManager : Capability {
        private readonly Dictionary<int, GameplayActor> roleActors = new();
        public IReadOnlyDictionary<int, GameplayActor> RoleActors => roleActors;
        
        protected override void OnSetup() {
            HeroNet.OnServerSpawn += OnRoleNetSpawn;
            HeroNet.OnServerDespawn += OnRoleNetDespawn;
        }

        protected override void OnDestroy() {
            HeroNet.OnServerSpawn -= OnRoleNetSpawn;
            HeroNet.OnServerDespawn -= OnRoleNetDespawn;
        }

        private void OnRoleNetSpawn(IServerHeroNet net) {
            var id = net.GetId();
            var character = World.CreateActor(ActorGroup.Player);
            character.SetupData(new HeroIdentifier() {
                IsControlTarget = false,
                PlayerId = net.GetId(),
                PlayerName = $"Player:{net.GetId()}"
            });
            character.SetupData(new HeroInputData());
            character.SetupData(new HeroMoveData());
            character.SetupCapability<CharacterMovement>(TickGroup.PlayerLogic);
            //character.SetupCapability<CharacterView>(TickGroup.FrameStep);
            roleActors.Add(id, character);
        }

        private void OnRoleNetDespawn(IServerHeroNet net) {
            if (roleActors.Remove(net.GetId(), out var character)) {
                character.Destroy();
            }
        }
    }
}