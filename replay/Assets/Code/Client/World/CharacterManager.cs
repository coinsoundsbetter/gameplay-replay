using System.Collections.Generic;

namespace KillCam.Client {
    public class CharacterManager : Feature {
        private readonly IRoleSpawnProvider provider;
        private readonly Dictionary<int, Character> characters = new();
        public IReadOnlyDictionary<int, Character> Characters => characters;

        public CharacterManager(IRoleSpawnProvider provider) {
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

        private void OnRoleSpawn(IClientCharacterNet net) {
            var playerId = net.GetId();
            if (characters.ContainsKey(playerId)) {
                return;
            }

            var characterActor = new Character() {
                Net = net,
            };
            characterActor.SetupWorld(world);
            characterActor.SetupData(new CharacterIdentifier() {
                PlayerId = playerId,
                IsControlTarget = net.IsClientOwned(),
                PlayerName = $"玩家{playerId}"
            });
            characterActor.SetupData(new CharacterInputData());
            characterActor.SetupData(new CharacterStateData());
            characterActor.SetupCapability<CharacterInput>(TickGroup.FixedStep);
            characterActor.SetupCapability<CharacterMovement>(TickGroup.FixedStep);
            characterActor.SetupCapability<CharacterView>(TickGroup.FrameStep);
            Get<ActorManager>().RegisterActor(characterActor);
            characters.Add(playerId, characterActor);
        }

        private void OnRoleDespawn(ICharacterNet net) {
            var playerId = net.GetId();
            if (characters.Remove(playerId, out var character)) {
                character.OnOwnerDestroyed();
                Get<ActorManager>().UnregisterActor(character);
            }
        }
    }
}