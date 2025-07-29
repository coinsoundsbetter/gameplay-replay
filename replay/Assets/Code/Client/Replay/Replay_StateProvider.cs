namespace KillCam.Client.Replay {
    public class Replay_StateProvider : Feature {
        public void SetState(S2C_Replay_WorldStateSnapshot snapshot) {
            var characters = Get<CharacterManager>().Characters;
            if (snapshot.CharacterSnapshot.StateData.IsEmpty) {
                return;
            }

            foreach (var kvp in snapshot.CharacterSnapshot.StateData) {
                ref var stateData = ref characters[kvp.Key].GetDataReadWrite<CharacterStateData>();
                stateData.Pos = kvp.Value.Pos;
                stateData.Rot = kvp.Value.Rot;
            }
        }
    }
}