using UnityEngine;

namespace KillCam {
    public abstract class HeroMomentMethod : SystemBase {
        protected void SimulateMove(ref Vector3 pos, Quaternion rot, 
            Vector2Int move, float speed, float deltaTime) {
            var forward = (rot * Vector3.forward).normalized;
            var right = (rot * Vector3.right).normalized;
            var moveDir = forward * move.y + right * move.x;
            var motion = moveDir * speed * deltaTime;
            pos += motion;
        }
    }
}