using UnityEngine;

namespace KillCam.Client {
    public class CharacterView : RoleView {
        private UCharacter viewBinder;
        private CameraManager cameraManager;
        private CameraDataSource association;
        
        protected override void OnSetup() {
            cameraManager = World.Get<CameraManager>();
        }

        protected override void OnActivate() {
            var cState = Owner.GetDataReadOnly<CharacterStateData>();
            Load(cState.Pos, cState.Rot);
        }

        protected override void OnDeactivate() {
            Unload();
        }

        protected override void OnTickActive() {
            RotateBody();
            UpdateCameraPitch();
            ApplyPos();
        }

        private void Load(Vector3 pos, Quaternion rot) {
            var asset = Resources.Load("Client/UCharacter");
            var instance = (GameObject)Object.Instantiate(asset);
            instance.SetLayerRecursively(World.HasFlag(WorldFlag.Replay)
                ? LayerDefine.replayCharacterLayer
                : LayerDefine.characterLayer);
            viewBinder = instance.GetComponent<UCharacter>();
            viewBinder.transform.position = pos;
            viewBinder.transform.rotation = rot;
            cameraManager.SetDataSource(association = new CameraDataSource() {
                pitch = 20f,
                pitchRange = new Vector2(-40f, 55f),
                yaw = 0,
                offsetFromTarget = new Vector3(0, 3, -5),
                target = viewBinder.cameraTarget,
            }); 
        }

        private void Unload() {
            if (viewBinder) {
                Object.Destroy(viewBinder.gameObject);
            }
        }

        private void RotateBody() {
            var inputData = Owner.GetDataReadOnly<CharacterInputData>();
            var mouseX = inputData.Yaw;
            viewBinder.transform.Rotate(Vector3.up, mouseX * (float)TickDelta * 300f, Space.Self);
            ref var stateData = ref Owner.GetDataReadWrite<CharacterStateData>();
            stateData.Rot = viewBinder.transform.rotation;
        }

        private void UpdateCameraPitch() {
            if (association == null) {
                return;
            }
            
            var inputData = Owner.GetDataReadOnly<CharacterInputData>();
            var mouseY = inputData.Pitch;
            association.pitch += -mouseY * (float)TickDelta * 300f;
        }

        private void ApplyPos() {
            var stateData = Owner.GetDataReadOnly<CharacterStateData>();
            viewBinder.transform.position = stateData.Pos;
        }
    }
}