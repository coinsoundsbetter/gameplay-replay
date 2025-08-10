using System.Collections.Generic;
using UnityEngine;

namespace KillCam.Client {
    public class Projectiles : Feature {
        private readonly List<ProjectileInstance> flyingProjectiles = new();
        private readonly List<GameObject> bulletObjects = new();

        protected override void OnTickActive() {
            for (int i = flyingProjectiles.Count - 1; i >= 0; i--) {
                var proj = flyingProjectiles[i];
                var nowSpeed = proj.speed;
                // 更新速度
                nowSpeed += Vector3.down * proj.gravity * (float)TickDelta;
                // 子弹位移
                var motion = nowSpeed * (float)TickDelta;
                // 子弹实体移动
                proj.objHandle.transform.position += motion;
                // 控制子弹实体最大距离
                proj.hasFlyDis += motion.magnitude;
                if (proj.hasFlyDis >= proj.totalFlyDis) {
                    ReturnBulletObject(proj.objHandle);
                    proj.objHandle = null;
                    flyingProjectiles.RemoveAt(i);
                }

                proj.speed = nowSpeed;
            }
        }

        public void SpawnBullet(BulletInitData data) {
            flyingProjectiles.Add(new ProjectileInstance() {
                speed = data.direction * data.burstSpeed,
                hasFlyDis = 0,
                gravity = 9.8f,
                totalFlyDis = data.maxDistance,
                objHandle = GetBulletObject(),
            });
        }
        
        private GameObject GetBulletObject() {
            if (bulletObjects.Count == 0) {
                return Object.Instantiate(Resources.Load<GameObject>("Bullet"));
            }
            
            var res = bulletObjects[0];
            bulletObjects.RemoveAt(0);
            return res;
        }

        private void ReturnBulletObject(GameObject obj) {
            bulletObjects.Add(obj);
        }
    }
}