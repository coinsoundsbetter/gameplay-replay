namespace KillCam.Client.Replay {
    public class Replay_StateProvider : Feature {
        public void SetState(S2C_WorldStateSnapshot snapshot) {
            var characters = GetSingletonFeature<Client_SpawnHeroSystem>().Characters;
            if (snapshot.HeroSnapshot.StateData.IsEmpty) {
                return;
            }

            foreach (var kvp in snapshot.HeroSnapshot.StateData) {
                ref var stateData = ref characters[kvp.Key].GetDataReadWrite<HeroMoveData>();
                stateData.Pos = kvp.Value.Pos;
                stateData.Rot = kvp.Value.Rot;
            }
        }
    }
}