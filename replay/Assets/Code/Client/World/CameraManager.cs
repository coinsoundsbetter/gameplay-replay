using UnityEngine;

namespace KillCam.Client {
    public class CameraManager : Feature {
        private Camera uCamera;
        private Transform uCameraTrans => uCamera.transform;
        private CameraDataSource ass;

        public override void OnCreate() {
            LoadCamera();
            world.AddFrameUpdate(OnFrameTick);
        }

        public override void OnDestroy() {
            world.RemoveFrameUpdate(OnFrameTick);
        }

        private void OnFrameTick(float delta) {
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
            if (world.HasFlag(WorldFlag.Replay)) {
                uCamera.cullingMask &= ~ (1 << LayerDefine.characterLayer);
            }
            else {
                uCamera.cullingMask |= (1 << LayerDefine.replayCharacterLayer);
            }
        }

        private void CheckEnable() {
            // 非回放世界
            bool isBlock = !world.HasFlag(WorldFlag.Replay) &&
                           (ClientData.Instance.IsReplaying || ClientData.Instance.IsReplayPrepare);

            // 回放世界
            if (world.HasFlag(WorldFlag.Replay) &&
                (ClientData.Instance.IsReplaying)) {
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
            if (lookDir.sqrMagnitude > 0.001f)
            {
                uCameraTrans.rotation = Quaternion.LookRotation(lookDir.normalized);
            }
        }

        private void SendCameraData() {
            world.Send(new C2S_SendCameraData() {
                Rotation = uCamera.transform.rotation,
            });
        }

        public CameraData GetCameraData() {
            var data = new CameraData() {
                lookForward = uCamera.transform.forward,
                forwardNoY = new Vector3(uCamera.transform.forward.x, 0, uCamera.transform.forward.z),
            };

            return data;
        }
    }

    public struct CameraData {
        public Vector3 lookForward;
        public Vector3 forwardNoY;
    }

    public class CameraDataSource {
        public Transform target;
        public Vector3 offsetFromTarget;
        public float pitch;
        public float yaw;
        public Vector2 pitchRange;
    }
}