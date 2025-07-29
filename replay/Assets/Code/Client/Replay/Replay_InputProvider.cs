using UnityEngine;

namespace KillCam.Client.Replay {
    public class Replay_InputProvider : Feature {
        public void SetInput(S2C_WorldStateSnapshot snapshot) {
            var characters = Get<CharacterManager>().Characters;
            if (snapshot.CharacterSnapshot.InputData.IsEmpty) {
                return;
            }

            foreach (var kvp in snapshot.CharacterSnapshot.InputData) {
                ref var data = ref characters[kvp.Key].GetDataReadWrite<CharacterInputData>();
                data.Move = kvp.Value.Move;
            }
        }
    }
}