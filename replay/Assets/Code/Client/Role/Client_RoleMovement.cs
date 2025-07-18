using UnityEngine;

namespace KillCam.Client
{
    public class Client_RoleMovement : RoleMovement
    {
        private Client_RoleInput input;

        public Vector3 Pos;
        public Quaternion Rotation;
        
        public Client_RoleMovement(Client_RoleInput input)
        {
            this.input = input;
        }
        
        public void Update(double delta)
        {
            Debug.Log("Input " + input.MoveInput);
            SimulateMove(ref Pos, Rotation, input.MoveInput, (float)delta);
        }
    }
}