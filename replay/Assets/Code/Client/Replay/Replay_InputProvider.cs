using UnityEngine;

namespace KillCam.Client.Replay {
    public class Replay_InputProvider : SystemBase {
        public void SetInput(S2C_WorldStateSnapshot snapshot) {
            var characters = GetSingletonFeature<Client_SpawnHeroSystem>().Characters;
            if (snapshot.HeroSnapshot.InputData.IsEmpty) {
                return;
            }

            foreach (var kvp in snapshot.HeroSnapshot.InputData) {
                ref var data = ref characters[kvp.Key].GetDataReadWrite<HeroInputState>();
                data.Move = kvp.Value.Move;
            }
        }
    }
}