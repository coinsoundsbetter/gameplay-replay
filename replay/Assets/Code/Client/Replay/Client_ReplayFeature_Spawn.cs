namespace KillCam.Client.Replay
{
    public class Client_ReplayFeature_Spawn : Feature
    {
        public override void OnCreate()
        {
            world.AddLogicUpdate(OnLogicUpdate);
        }

        public override void OnDestroy()
        {
            world.RemoveLogicUpdate(OnLogicUpdate);
        }

        private void OnLogicUpdate(double delta)
        {
            
        }
    }
}