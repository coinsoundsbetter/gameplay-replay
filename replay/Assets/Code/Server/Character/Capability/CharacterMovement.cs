namespace KillCam.Server
{
    public class CharacterMovement : RoleMovement
    {
        public override bool OnShouldActivate()
        {
            ref var inputData = ref Owner.GetDataReadWrite<CharacterInputData>();
            return inputData.HasValidInput();
        }

        public override bool OnShouldDeactivate()
        {
            ref var inputData = ref Owner.GetDataReadWrite<CharacterInputData>();
            return !inputData.HasValidInput();
        }

        protected override void OnTickActive()
        {
            var inputData = Owner.GetDataReadOnly<CharacterInputData>();
            ref var stateData = ref Owner.GetDataReadWrite<CharacterStateData>();
            SimulateMove(ref stateData.Pos, stateData.Rot, inputData.Move, (float)TickDelta);
        }
    }
}