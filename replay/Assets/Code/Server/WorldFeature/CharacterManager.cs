using System.Collections.Generic;

namespace KillCam.Server {
    public class CharacterManager : Feature {
        private readonly Dictionary<int, Character> roleActors = new();
        public IReadOnlyDictionary<int, Character> RoleActors => roleActors;

        public override void OnCreate() {
            CharacterNet.OnServerSpawn += OnRoleNetSpawn;
            CharacterNet.OnServerDespawn += OnRoleNetDespawn;
        }

        public override void OnDestroy() {
            CharacterNet.OnServerSpawn -= OnRoleNetSpawn;
            CharacterNet.OnServerDespawn -= OnRoleNetDespawn;
        }

        private void OnRoleNetSpawn(IServerCharacterNet net) {
            var id = net.GetId();
            var character = new Character() {
                Net = net,
            };
            character.SetupWorld(world);
            character.SetupData(new CharacterIdentifier() {
                IsControlTarget = false,
                PlayerId = net.GetId(),
                PlayerName = $"Player:{net.GetId()}"
            });
            character.SetupData(new CharacterInputData());
            character.SetupData(new CharacterStateData());
            character.SetupCapability<CharacterMovement>(TickGroup.FixedStep);
            character.SetupCapability<CharacterView>(TickGroup.FrameStep);
            Get<ActorManager>().RegisterActor(character);
            roleActors.Add(id, character);
        }

        private void OnRoleNetDespawn(IServerCharacterNet net) {
            if (roleActors.Remove(net.GetId(), out var character)) {
                character.OnOwnerDestroyed();
                Get<ActorManager>().UnregisterActor(character);
            }
        }
    }
}