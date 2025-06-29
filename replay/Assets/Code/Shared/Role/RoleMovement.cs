using UnityEngine;

namespace KillCam
{
    public abstract class RoleMovement
    {
        protected void SimulateMove(ref Vector3 pos, Quaternion rot, Vector2 move, float deltaTime)
        {
            var forward = (rot * Vector3.forward).normalized;
            var right = (rot * Vector3.right).normalized;
            var moveDir = forward * move.y + right * move.x;
            var motion = moveDir * 3f * deltaTime;
            pos += motion;
        }
    }
}