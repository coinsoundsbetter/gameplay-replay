using UnityEngine;

namespace KillCam.Client {
    public class CameraManager : Feature {
        private Camera uCamera;
        private CameraTargetAssociation ass;

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
        
        private void LoadCamera() {
            var asset = Resources.Load("Camera");
            var go = (GameObject)Object.Instantiate(asset);
            uCamera = go.GetComponent<Camera>();
            uCamera.transform.position = new Vector3(0, 0, -10);
            if (world.HasFlag(WorldFlag.Replay)) {
                uCamera.cullingMask &= ~ (1 << LayerDefine.characterLayer);
            } else {
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

            // 1. 计算目标位置（使用本地 offset ）
            Vector3 desiredPos = ass.target.position + ass.target.rotation * ass.offsetInLocal;

            // 2. 平滑插值到目标位置
            uCamera.transform.position = Vector3.Lerp(uCamera.transform.position, desiredPos, FrameDelta * ass.followSpeed);

            // 3. 计算朝向
            Vector3 lookDir = ass.target.position - uCamera.transform.position;
            if (lookDir.sqrMagnitude > 0.001f) {
                Quaternion desiredRot = Quaternion.LookRotation(lookDir.normalized);
                uCamera.transform.rotation = Quaternion.Slerp(uCamera.transform.rotation, desiredRot, FrameDelta * ass.rotateSpeed);
            }
        }

        private void SendCameraData() {
            world.Send(new C2S_SendCameraData() {
                Rotation = uCamera.transform.rotation,
            });
        }

        public void SetFollowTarget(CameraTargetAssociation association) {
            ass = association;
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

    public class CameraTargetAssociation {
        public Transform target;
        public Vector3 offsetInLocal;
        public float followSpeed;
        public float rotateSpeed;
    }
}