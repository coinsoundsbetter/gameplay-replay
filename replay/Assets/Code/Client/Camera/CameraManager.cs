using UnityEngine;

namespace KillCam.Client {
    public class CameraManager : Feature {
        private Camera uCamera;
        private Transform uCameraTrans => uCamera.transform;
        private CameraDataSource ass;

        protected override void OnCreate() {
            LoadCamera();
        }

        protected override void OnTickActive() {
            CheckEnable();
            Movement();
            SendCameraData();
        }

        public void SetDataSource(CameraDataSource association) {
            ass = association;
        }

        private void LoadCamera() {
            var asset = Resources.Load("Camera");
            var go = (GameObject)Object.Instantiate(asset);
            uCamera = go.GetComponent<Camera>();
            uCamera.transform.position = new Vector3(0, 0, -10);
            if (HasWorldFlag(WorldFlag.Replay)) {
                uCamera.cullingMask &= ~ (1 << LayerDefine.characterLayer);
            }else {
                uCamera.cullingMask |= (1 << LayerDefine.replayCharacterLayer);
            }
        }

        private void CheckEnable() {
            // 非回放世界
            bool isBlock = !HasWorldFlag(WorldFlag.Replay) &&
                           (AppData.Instance.IsReplaying || AppData.Instance.IsReplayPrepare);

            // 回放世界
            if (HasWorldFlag(WorldFlag.Replay) &&
                (AppData.Instance.IsReplaying)) {
                isBlock = false;
            }

            uCamera.enabled = !isBlock;
        }

        private void Movement() {
            if (ass == null || ass.target == null) {
                return;
            }

            // 限制俯仰角
            ass.pitch = Mathf.Clamp(ass.pitch, ass.pitchRange.x, ass.pitchRange.y);

            // 1. 水平旋转：绕 Y 轴旋转（yaw）
            Quaternion yawRotation = Quaternion.Euler(0f, ass.target.eulerAngles.y, 0f);

            // 2. 俯仰旋转：绕 X 轴旋转（pitch）
            Quaternion pitchRotation = Quaternion.Euler(ass.pitch, 0f, 0f);

            // 3. 先水平旋转，再俯仰旋转（先绕Y，再绕X）
            Vector3 rotatedOffset = yawRotation * pitchRotation * ass.offsetFromTarget;

            // 4. 计算相机目标位置
            Vector3 desiredPos = ass.target.position + rotatedOffset;

            // 5. 设定相机位置
            uCameraTrans.position = desiredPos;

            // 6. 相机始终看向角色
            Vector3 lookDir = ass.target.position - uCameraTrans.position;
            if (lookDir.sqrMagnitude > 0.001f) {
                uCameraTrans.rotation = Quaternion.LookRotation(lookDir.normalized);
            }
        }

        private void SendCameraData() {
            ref var camData = ref GetWorldDataRW<CameraData>();
            camData.aimDirection = GetCameraAimDirection();
            camData.aimTarget = GetCameraAimTarget();
            Send(new C2S_SendCameraData() {
                Rotation = uCamera.transform.rotation,
            });
        }

        private Vector3 GetCameraAimDirection(float maxDistance = 1000f, LayerMask mask = default) {
            Camera cam = uCamera;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f)); // 屏幕中心
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask)) {
                return (hit.point - cam.transform.position).normalized;
            }

            return ray.direction; // 没打到东西就直接 forward
        }

        private Vector3 GetCameraAimTarget(float maxDistance = 1000f, LayerMask hitMask = default) {
            Ray ray = uCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // 从屏幕中心射出
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask)) {
                return hit.point; // 命中物体
            }
            else {
                return ray.origin + ray.direction * maxDistance; // 没命中则取远处一点
            }
        }
    }

    public struct CameraData {
        public Vector3 lookForward;
        public Vector3 forwardNoY;
        public Vector3 aimDirection;
        public Vector3 aimTarget;
    }

    public class CameraDataSource {
        public Transform target;
        public Vector3 offsetFromTarget;
        public float pitch;
        public float yaw;
        public Vector2 pitchRange;
    }
}