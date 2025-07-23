using System.Collections.Generic;

namespace KillCam.Client
{
    public class Client_RoleManager : Feature
    {
        private readonly IRoleSpawnProvider provider;
        private readonly Dictionary<int, Client_Character> characters = new();
        public IReadOnlyDictionary<int, Client_Character> Characters => characters;
        
        public Client_RoleManager(IRoleSpawnProvider provider)
        {
            this.provider = provider;
        }

        public override void OnCreate()
        {
            provider.OnRoleSpawn += OnRoleSpawn;
            provider.OnRoleDespawn += OnRoleDespawn;
        }

        public override void OnDestroy()
        {
            provider.OnRoleSpawn -= OnRoleSpawn;
            provider.OnRoleDespawn -= OnRoleDespawn;
        }

        private void OnRoleSpawn(IClientRoleNet net)
        {
            var playerId = net.GetId();
            if (characters.ContainsKey(playerId))
            {
                return;
            }

            var characterActor = new Client_Character()
            {
                Net = net,
            };
            characterActor.SetupWorld(world);
            characterActor.SetupData(new CharacterIdentifier()
            {
                PlayerId = playerId,
                IsControlTarget = net.IsClientOwned(),
                PlayerName = $"玩家{playerId}"
            });
            characterActor.SetupData(new CharacterInputData());
            characterActor.SetupData(new CharacterStateData());
            characterActor.SetupCapability<Client_CharacterInput>(TickGroup.FixedStep);
            characterActor.SetupCapability<Client_CharacterMovement>(TickGroup.FixedStep);
            characterActor.SetupCapability<Client_CharacterView>(TickGroup.FrameStep);
            Get<ActorManager>().RegisterActor(characterActor);
            characters.Add(playerId, characterActor);
        }
        
        private void OnRoleDespawn(IRoleNet net)
        {
            var playerId = net.GetId();
            if (characters.Remove(playerId, out var character))
            {
                character.OnOwnerDestroyed();
                Get<ActorManager>().UnregisterActor(character);
            }
        }
    }
}