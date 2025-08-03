using UnityEngine;

namespace KillCam.Client {
    public class HeroMovement : HeroMomentMethod {

        protected override void OnTickActive() {
            RotateBody();
            MoveStep();
        }

        private void RotateBody() {
            ref var moveData = ref Owner.GetDataReadWrite<HeroMoveData>();
            var inputData = Owner.GetDataReadOnly<HeroInputData>();
            var mouseX = inputData.Yaw;
            var deltaRotation = Quaternion.AngleAxis(mouseX * (float)TickDelta * 300f, Vector3.up);
            moveData.Rot *= deltaRotation;
        }

        private void MoveStep() {
            ref var moveData = ref Owner.GetDataReadWrite<HeroMoveData>();
            var inputData = Owner.GetDataReadOnly<HeroInputData>();
            var lastPos = moveData.Pos;
            SimulateMove(ref moveData.Pos, moveData.Rot, inputData.Move, 3f, (float)TickDelta);
            moveData.IsMoving = lastPos != moveData.Pos;
            moveData.WorldMoveDirection = (moveData.Pos - lastPos).normalized;
            moveData.LocalMoveDirection = Quaternion.Inverse(moveData.Rot) * moveData.WorldMoveDirection;
        }
    }
}