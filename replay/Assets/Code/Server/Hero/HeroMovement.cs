namespace KillCam.Server {
    public class HeroMovement : HeroMomentMethod {
        public override bool OnShouldActivate() {
            ref var inputData = ref GetDataRef<HeroInputState>();
            return inputData.HasValidInput();
        }

        protected override void OnTick() {
            var inputData = GetDataRO<HeroInputState>();
            ref var moveData = ref GetDataRef<HeroMoveData>();
            var lastPos = moveData.Pos;
            SimulateMove(ref moveData.Pos, moveData.Rot, inputData.Move, 3f, (float)TickDeltaDouble);
            moveData.IsMoving = lastPos != moveData.Pos;
        }
    }
}