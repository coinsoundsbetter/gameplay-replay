namespace KillCam.Server {
    public class CharacterMovement : HeroMomentMethod {
        public override bool OnShouldTick() {
            ref var inputData = ref Owner.GetDataReadWrite<HeroInputData>();
            return inputData.HasValidInput();
        }

        protected override void OnTickActive() {
            var inputData = Owner.GetDataReadOnly<HeroInputData>();
            ref var moveData = ref Owner.GetDataReadWrite<HeroMoveData>();
            var lastPos = moveData.Pos;
            SimulateMove(ref moveData.Pos, moveData.Rot, inputData.Move, 3f, (float)TickDelta);
            moveData.IsMoving = lastPos != moveData.Pos;
        }
    }
}