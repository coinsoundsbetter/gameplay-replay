namespace KillCam.Client.Replay
{
    public class Replay_InputProvider : Feature
    {
        public void SetInput(S2C_Replay_WorldStateSnapshot snapshot)
        {
            foreach (var kvp in snapshot.RoleStateSnapshots)
            {
                var dict = world.Get<Client_RoleManager>().RoleLogics;
                var role = dict[kvp.Key];
                role.Input.SetInputData(kvp.Value.MoveInput.x, kvp.Value.MoveInput.y);
            }
        }
    }
}