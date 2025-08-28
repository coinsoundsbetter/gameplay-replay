using UnityEngine;

namespace KillCam.Client {
    public class HeroMovement : HeroMomentMethod {

        protected override void OnTick() {
            RotateBody();
            MoveStep();
        }

        private void RotateBody() {
            ref var moveData = ref GetDataRef<HeroMoveData>();
            var inputData = GetDataRO<HeroInputState>();
            var mouseX = inputData.Yaw;
            var deltaRotation = Quaternion.AngleAxis(mouseX * (float)TickDeltaDouble * 300f, Vector3.up);
            moveData.Rot *= deltaRotation;
        }

        private void MoveStep() {
            ref var moveData = ref GetDataRef<HeroMoveData>();
            var inputData = GetDataRO<HeroInputState>();
            var lastPos = moveData.Pos;
            SimulateMove(ref moveData.Pos, moveData.Rot, inputData.Move, 3f, (float)TickDeltaDouble);
            moveData.IsMoving = lastPos != moveData.Pos;
            moveData.WorldMoveDirection = (moveData.Pos - lastPos).normalized;
            moveData.LocalMoveDirection = Quaternion.Inverse(moveData.Rot) * moveData.WorldMoveDirection;
        }
    }
}