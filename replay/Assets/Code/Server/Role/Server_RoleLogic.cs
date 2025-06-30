using UnityEngine;

namespace KillCam.Server
{
    public class Server_RoleLogic : RoleLogic
    {
        private BattleWorld myWorld;
        public Server_RoleInput Input { get; private set; }
        public Server_RoleMovement Movement { get; private set; }
        public Server_RoleView View { get; private set; }
        
        public RoleStateSnapshot NetStateData { get; private set; }

        public void Init(BattleWorld world)
        {
            myWorld = world;
            Movement = new Server_RoleMovement();
            View = new Server_RoleView(Movement);
            Input = new Server_RoleInput(Movement);
            View.Load(Vector3.zero, Quaternion.identity);
        }

        public void Clear()
        {
            View.Unload();
        }

        public void TickLogic(double delta)
        {
            Input.Update(delta);
        }

        public void TickFrame(float delta)
        {
            View.Update(delta);
        }
    }
}