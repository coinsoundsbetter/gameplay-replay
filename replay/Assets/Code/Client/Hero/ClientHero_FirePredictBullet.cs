using Unity.Mathematics;

namespace KillCam.Client {
    /// <summary>
    /// 本地预测生成子弹
    /// </summary>
    public class ClientHero_FirePredictBullet : SystemBase {
        
        protected override void OnTick() {
            ref var intents = ref GetBuffer<HeroFireIntentElem>();
            if (intents.Length == 0) {
                return;
            }

            var networkTime = GetWorldData<NetworkTime>();
            var identifier = GetDataRO<HeroIdentifier>();
            for (int i = 0; i < intents.Length; i++) {
                var intent = intents[i];
                var shotId = ShotIdUtils.Compute(identifier.PlayerId, intent.LocalShotSeq, networkTime.Tick);
                float3 vel = math.normalize(intent.Dir) * 300f;
                SpawnPredictBullet(identifier.PlayerId, shotId, intent.Origin, vel);
            }
            intents.Clear();
        }

        private void SpawnPredictBullet(int ownerId, uint shotId, float3 pos, float3 vel) {
            var actor = CreateActor(ActorGroup.Bullet);
            actor.AddData(new PredictState() {
                IsInPredict = true,
            });
            actor.AddData(new BulletState() {
                Position = pos,
                Velocity = vel,
                OwnerId = ownerId,
                ShotId = shotId,
            });
        }
    }
}