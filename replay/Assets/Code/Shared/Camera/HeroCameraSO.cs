using UnityEngine;

namespace KillCam {
    [CreateAssetMenu(menuName = "玩法/角色相机配置", fileName = "HeroCameraSO")]
    public class HeroCameraSO : ScriptableObject {
        public float initPitch = 20f;
        public float initYaw = 0f;
        public Vector2 pitchRange = new Vector2(-40f, 55f);
        public Vector3 offsetFromTarget = new Vector3(0, 3, -5);
    }
}