using UnityEngine;

namespace KillCam.Client {
    public class CharacterView : RoleView {
        private Mono_Role role;
        private CameraManager cameraManager;
        private CameraTargetAssociation association;
        
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
            var cState = Owner.GetDataReadOnly<CharacterStateData>();
            role.transform.position = cState.Pos;
            role.transform.rotation = cState.Rot;
            AnimationState();
        }

        private void Load(Vector3 pos, Quaternion rot) {
            var asset = Resources.Load("Client_Mono_Role");
            var instance = (GameObject)Object.Instantiate(asset);
            instance.SetLayerRecursively(World.HasFlag(WorldFlag.Replay)
                ? LayerDefine.replayCharacterLayer
                : LayerDefine.characterLayer);
            role = instance.GetComponent<Mono_Role>();
            role.transform.position = pos;
            role.transform.rotation = rot;
            cameraManager.SetFollowTarget(association = new CameraTargetAssociation() {
                followSpeed = 10f,
                rotateSpeed = 30f,
                offsetInLocal = new Vector3(0, 3, -5),
                target = role.cameraTarget,
            }); 
        }

        private void Unload() {
            if (role) {
                Object.Destroy(role.gameObject);
            }
        }

        private void AnimationState() {
            var inputState = Owner.GetDataReadOnly<CharacterInputData>();
            var move = inputState.Move;
            
        }
    }
}