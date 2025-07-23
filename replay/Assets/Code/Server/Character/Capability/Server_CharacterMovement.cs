namespace KillCam.Server
{
    public class Server_CharacterMovement : RoleMovement
    {
        public override bool OnShouldActivate()
        {
            ref var inputData = ref Owner.GetDataReadWrite<CharacterInputData>();
            return inputData.IsValid;
        }

        public override bool OnShouldDeactivate()
        {
            ref var inputData = ref Owner.GetDataReadWrite<CharacterInputData>();
            return !inputData.IsValid;
        }

        protected override void OnTickActive()
        {
            var inputData = Owner.GetDataReadOnly<CharacterInputData>();
            ref var stateData = ref Owner.GetDataReadWrite<CharacterStateData>();
            SimulateMove(ref stateData.Pos, stateData.Rot, inputData.Move, (float)TickDelta);
        }
    }
}