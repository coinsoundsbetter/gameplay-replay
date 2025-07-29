using UnityEngine;

namespace KillCam.Client {
    public class CharacterView : RoleView {
        private Mono_Role role;

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
        }

        private void Unload() {
            if (role) {
                Object.Destroy(role.gameObject);
            }
        }

        private void AnimationState() {
        }
    }
}