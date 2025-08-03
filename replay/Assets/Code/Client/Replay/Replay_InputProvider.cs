using UnityEngine;

namespace KillCam.Client.Replay {
    public class Replay_InputProvider : Feature {
        public void SetInput(S2C_WorldStateSnapshot snapshot) {
            var characters = Get<HeroManager>().Characters;
            if (snapshot.HeroSnapshot.InputData.IsEmpty) {
                return;
            }

            foreach (var kvp in snapshot.HeroSnapshot.InputData) {
                ref var data = ref characters[kvp.Key].GetDataReadWrite<HeroInputData>();
                data.Move = kvp.Value.Move;
            }
        }
    }
}