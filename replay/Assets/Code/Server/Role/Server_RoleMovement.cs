using UnityEngine;

namespace KillCam.Server
{
    public class Server_RoleMovement : RoleMovement
    {
        public Vector3 Pos;
        public Quaternion Rotation;
        
        public void ApplyInput(Vector2 input, float delta)
        {
            SimulateMove(ref Pos, Rotation, input, delta);    
        }
    }
}