using UnityEngine;

namespace KillCam.Client {
    public class CameraManager : Feature {
        private Camera uCamera;

        public override void OnCreate() {
            LoadCamera();
            world.AddFrameUpdate(OnFrameTick);
        }

        public override void OnDestroy() {
            world.RemoveFrameUpdate(OnFrameTick);
        }

        private void OnFrameTick(float delta) {
            bool isBlock = false;

            // 非回放世界
            if (!world.HasFlag(WorldFlag.Replay) &&
                (ClientData.Instance.IsReplaying || ClientData.Instance.IsReplayPrepare)) {
                isBlock = true;
            }

            // 回放世界
            if (world.HasFlag(WorldFlag.Replay) &&
                (ClientData.Instance.IsReplaying)) {
                isBlock = false;
            }

            uCamera.enabled = !isBlock;
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
    }
}