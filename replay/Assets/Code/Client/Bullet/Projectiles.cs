using System.Collections.Generic;
using UnityEngine;

namespace KillCam.Client {
    public class Projectiles : Feature {
        private readonly List<ProjectileInstance> flyingProjectiles = new();
        private readonly List<GameObject> bulletObjects = new();

        protected override void OnTick() {
            for (int i = flyingProjectiles.Count - 1; i >= 0; i--) {
                var proj = flyingProjectiles[i];
                var lastPos = proj.nowPos;
                proj.speed += Vector3.down * proj.gravity * (float)TickDeltaDouble;
                proj.nowPos += proj.speed * (float)TickDeltaDouble;
                proj.objHandle.transform.position = proj.nowPos;
                proj.hasFlyDis += Vector3.Distance(proj.nowPos, lastPos);
                if (proj.hasFlyDis >= proj.totalFlyDis) {
                    ReturnBulletObject(proj.objHandle);
                    proj.objHandle = null;
                    flyingProjectiles.RemoveAt(i);
                }else {
                    flyingProjectiles[i] = proj;
                }
            }
        }

        public void SpawnBullet(BulletInitData data) {
            flyingProjectiles.Add(new ProjectileInstance() {
                nowPos = data.origin,
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
            res.SetActive(true);
            return res;
        }

        private void ReturnBulletObject(GameObject obj) {
            bulletObjects.Add(obj);
            obj.SetActive(false);
        }
    }
}