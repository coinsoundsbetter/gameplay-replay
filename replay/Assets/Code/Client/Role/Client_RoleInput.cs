using UnityEngine;

namespace KillCam.Client
{
    public class Client_RoleInput
    {
        private BattleWorld world;
        
        public Client_RoleInput(BattleWorld myWorld)
        {
            world = myWorld;
        }

        public Vector2 MoveInput { get; private set; }
        
        public void Update()
        {
            var h = Input.GetAxis("Horizontal");    
            var v = Input.GetAxis("Vertical");
            if (h > 0) h = 1; 
            else if (h < 0) h = -1;
            if (v > 0) v = 1; 
            else if (v < 0) v = -1;
            MoveInput = new Vector2(h, v);
            world.Send(new C2S_SendInput()
            {
                LocalTick = world.NetworkClient.GetTick(),
                Move = MoveInput,
            });
        }
    }
}