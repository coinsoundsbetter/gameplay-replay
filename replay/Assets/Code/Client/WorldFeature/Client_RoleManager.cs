using System.Collections.Generic;

namespace KillCam.Client
{
    public class Client_RoleManager : Feature
    {
        private readonly IRoleSpawnProvider provider;
        private readonly Dictionary<int, IRoleNet> roleNets = new();
        private readonly Dictionary<int, Client_RoleLogic> roleLogics = new();

        public Client_RoleManager(IRoleSpawnProvider provider)
        {
            this.provider = provider;
        }

        public override void OnCreate()
        {
            provider.OnRoleSpawn += OnRoleSpawn;
            provider.OnRoleDespawn += OnRoleDespawn;
            world.AddLogicUpdate(OnLogicTick);
            world.AddFrameUpdate(OnFrameRefresh);
        }

        public override void OnDestroy()
        {
            provider.OnRoleSpawn -= OnRoleSpawn;
            provider.OnRoleDespawn -= OnRoleDespawn;
        }
        
        private void OnLogicTick(double delta)
        {
            foreach (var logic in roleLogics.Values)
            {
                logic.TickLogic(delta);
            }
        }
        
        private void OnFrameRefresh(float delta)
        {
            foreach (var logic in roleLogics.Values)
            {
                logic.TickFrame(delta);
            }
        }

        private void OnRoleSpawn(IRoleNet net)
        {
            var id = net.GetId();
            var data = net.GetData();
            roleNets.Add(id, net);

            var newLogic = new Client_RoleLogic();
            newLogic.Init(world);
            roleLogics.Add(id, newLogic);
        }
        
        private void OnRoleDespawn(IRoleNet net)
        {
            var id = net.GetId();
            roleNets.Remove(id);
            if (roleLogics.Remove(id, out var logic))
            {
                logic.Clear();
            }
        }
    }
}