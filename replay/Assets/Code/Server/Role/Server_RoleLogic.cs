using UnityEngine;

namespace KillCam.Server
{
    public class Server_RoleLogic : RoleLogic
    {
        private BattleWorld myWorld;
        public Server_RoleInput Input { get; private set; }
        public Server_RoleMovement Movement { get; private set; }
        public Server_RoleView View { get; private set; }
        private RoleStateSnapshot netStateData;
        public RoleStateSnapshot GetNetStateData() => netStateData;

        public void Init(BattleWorld world)
        {
            myWorld = world;
            Movement = new Server_RoleMovement();
            View = new Server_RoleView(Movement);
            Input = new Server_RoleInput(Movement, myWorld);
            View.Load(Vector3.zero, Quaternion.identity);
        }

        public void Clear()
        {
            View.Unload();
        }

        public void TickLogic(double delta)
        {
            Input.Update(delta);
            UpdateNetState();
        }

        public void TickFrame(float delta)
        {
            View.Update(delta);
        }

        private void UpdateNetState()
        {
            netStateData.Pos = Movement.Pos;
            netStateData.Rot = Movement.Rotation;
            netStateData.MoveInput = Input.UseInputData.Move;
        }
    }
}