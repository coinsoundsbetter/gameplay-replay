using UnityEngine;

namespace KillCam.Client {
    public class CharacterView : RoleView {
        private CharacterViewBinder viewBinder;
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
            viewBinder.transform.position = cState.Pos;
            viewBinder.transform.rotation = cState.Rot;
            AnimationState();
        }

        private void Load(Vector3 pos, Quaternion rot) {
            var asset = Resources.Load("Client/UCharacter");
            var instance = (GameObject)Object.Instantiate(asset);
            instance.SetLayerRecursively(World.HasFlag(WorldFlag.Replay)
                ? LayerDefine.replayCharacterLayer
                : LayerDefine.characterLayer);
            viewBinder = instance.GetComponent<CharacterViewBinder>();
            viewBinder.transform.position = pos;
            viewBinder.transform.rotation = rot;
            cameraManager.SetFollowTarget(association = new CameraTargetAssociation() {
                followSpeed = 10f,
                rotateSpeed = 30f,
                offsetInLocal = new Vector3(0, 3, -5),
                target = viewBinder.cameraTarget,
            }); 
        }

        private void Unload() {
            if (viewBinder) {
                Object.Destroy(viewBinder.gameObject);
            }
        }

        private void AnimationState() {
            var inputState = Owner.GetDataReadOnly<CharacterInputData>();
            var move = inputState.Move;
            
        }
    }
}