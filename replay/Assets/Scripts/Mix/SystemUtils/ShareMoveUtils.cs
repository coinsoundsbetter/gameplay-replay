using UnityEngine;

namespace KillCam
{
    public class ShareMoveUtils
    {
        public static Vector3 GetNextPos(PlayerInputState inputState, Vector3 forward, float speed)
        {
            int forwardInt = 0;
            if (inputState.Move.y > 0)
            {
                forwardInt = 1;
            }
            else if(inputState.Move.y < 0)
            {
                forwardInt = -1;
            }

            int rightInt = 0;
            if (inputState.Move.x > 0)
            {
                rightInt = 1;
            }
            else if (inputState.Move.x < 0)
            {
                rightInt = -1;
            }
            var motion = new Vector3(forward.x * rightInt * speed, 0, forward.z * forwardInt * speed);
            return motion;
        }
        
    }
}