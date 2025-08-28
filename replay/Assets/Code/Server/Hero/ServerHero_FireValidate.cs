namespace KillCam.Server {
    /// <summary>
    /// 开火验证
    /// </summary>
    public class ServerHero_FireValidate : Feature {
        
        protected override void OnTick() {
            var tick = GetWorldData<NetworkTime>().Tick;
            var identifier = GetDataRO<HeroIdentifier>();
            ref var fireCmdBuffer = ref GetBuffer<C2S_CmdFire>();
            ref var wpnState = ref GetDataRef<HeroWeaponState>();
            while (fireCmdBuffer.Length > 0) {
                var fireCmd = fireCmdBuffer[0];
                fireCmdBuffer.RemoveAt(0);
                wpnState.LocalShotSeq++;
                var shotId = ShotIdUtils.Compute(identifier.PlayerId, wpnState.LocalShotSeq, tick);
                var bullet = CreateActor(ActorGroup.Bullet);
                bullet.AddData(new BulletState() {
                    OwnerId = identifier.PlayerId,
                    Position = fireCmd.FireOrigin,
                    Velocity = fireCmd.FireDir * 300,
                    Lifetime = 4,
                    ShotId = shotId,
                });
            }
        }
    }
}