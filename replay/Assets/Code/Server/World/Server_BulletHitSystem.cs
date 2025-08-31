namespace KillCam.Server {
    /// <summary>
    /// 服务器子弹命中
    /// </summary>
    public class Server_BulletHitSystem : SystemBase {
        
        protected override void OnTick() {
            foreach (var actor in GetActors(ActorGroup.Bullet)) {
                var sweep = actor.GetDataReadOnly<BulletSweep>();
            }
        }
    }
}