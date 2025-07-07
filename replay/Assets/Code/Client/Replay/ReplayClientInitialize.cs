
namespace KillCam.Client.Replay
{
    public class ReplayClientInitialize : Feature 
    {
        public override void OnCreate()
        {
            AddReplayFeatures(world);
        }

        private void AddReplayFeatures(BattleWorld w)
        {
            w.Add(new Client_ReplayFeature_State());
        }
    }
}