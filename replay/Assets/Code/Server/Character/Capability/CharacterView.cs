using UnityEngine;

namespace KillCam.Server {
    public class CharacterView : RoleView {
        private UCharacter viewBinder;

        protected override void OnActivate() {
            var state = Owner.GetDataReadOnly<CharacterStateData>();
            Load(state.Pos, state.Rot);
        }

        protected override void OnDeactivate() {
            Unload();
        }

        protected override void OnTickActive() {
            var state = Owner.GetDataReadOnly<CharacterStateData>();
            viewBinder.transform.position = state.Pos;
        }

        private void Load(Vector3 pos, Quaternion rot) {
            var asset = Resources.Load("Server/UCharacter");
            var instance = (GameObject)Object.Instantiate(asset);
            viewBinder = instance.GetComponent<UCharacter>();
            viewBinder.transform.position = pos;
            viewBinder.transform.rotation = rot;
        }

        private void Unload() {
            if (viewBinder) {
                Object.Destroy(viewBinder.gameObject);
            }
        }
    }
}