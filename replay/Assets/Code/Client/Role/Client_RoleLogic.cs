using UnityEngine;

namespace KillCam.Client
{
    public class Client_RoleLogic : RoleLogic
    {
        private BattleWorld myWorld;
        public Client_RoleInput Input { get; private set; }
        public Client_RoleMovement Movement { get; private set; }
        public Client_RoleView View { get; private set; }
        
        public void Init(BattleWorld world)
        {
            myWorld = world;
            Input = new Client_RoleInput(myWorld);
            Movement = new Client_RoleMovement(Input);
            View = new Client_RoleView(Movement);
            View.Load(Vector3.zero, Quaternion.identity);
        }

        public void Clear()
        {
            View.Unload();
        }

        public void TickLogic(double delta)
        {
            Input.Update();
            Movement.Update(delta);
        }

        public void TickFrame(float delta)
        {
            View.Update(delta);
        }
    }
}