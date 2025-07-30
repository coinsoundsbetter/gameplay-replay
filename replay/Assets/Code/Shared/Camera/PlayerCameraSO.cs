using UnityEngine;

namespace KillCam {
    [CreateAssetMenu(menuName = "玩法/角色相机配置", fileName = "PlayerCameraSO")]
    public class PlayerCameraSO : ScriptableObject {
        public Vector3 offset;
        public float rotateSpeed;
        public float followSpeed;
    }
}