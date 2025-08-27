namespace KillCam.Server {
    public class HeroFire : Feature {
        
        protected override void OnTickActive() {
            ref var fireCmdBuffer = ref GetBuffer<C2S_CmdFire>();
            if (fireCmdBuffer.Length == 0) {
                return;
            }

            var cmd = fireCmdBuffer[0];
            fireCmdBuffer.RemoveAt(0);
            ref var ack = ref GetDataRW<HeroFireAckData>();
            
            // 去重
            if (ack.AckFireIds.Contains(cmd.FireId)) {
                return;
            }
            
            if (cmd.FireId <= ack.LastAckFireId) {
                return;    
            }
            ack.LastAckFireId = cmd.FireId;
            
            // 是否处于开火冷却
            ref var weaponData = ref GetDataRW<HeroWeaponData>();
            if (weaponData.AllowFireTick < GetTick()) {
                return;
            }

            // 是否没有弹药了
            if (weaponData.AmmonInMag <= 0) {
                return;
            }
            
            BroadcastAll(new S2C_FireAck() {
                FireId = cmd.FireId,
                ServerFireTick = GetTick(),
                Accept = true,
                CauseDamage = 1,
            });
        }
    }
}