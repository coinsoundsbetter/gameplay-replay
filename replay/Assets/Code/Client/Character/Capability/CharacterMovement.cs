using UnityEngine;

namespace KillCam.Client
{
    public class CharacterMovement : RoleMovement
    {
        protected override void OnTickActive()
        {
            ref var moveData = ref Owner.GetDataReadWrite<CharacterStateData>();
            var inputData = Owner.GetDataReadOnly<CharacterInputData>();
            SimulateMove(ref moveData.Pos, moveData.Rot, inputData.Move, (float)TickDelta);
        }
    }
}