using UnityEngine;

namespace KillCam.Client {
    public class CharacterVisualSkin : Capability {
        private GameObject instance;
        
        protected override void OnActivate() {
            LoadSkin();
        }

        protected override void OnDeactivate() {
            ClearSkin();
        }

        private void LoadSkin() {
            var asset = Resources.Load("Client/UCharacter");
            instance = (GameObject)Object.Instantiate(asset);
            instance.SetLayerRecursively(World.HasFlag(WorldFlag.Replay)
                ? LayerDefine.replayCharacterLayer
                : LayerDefine.characterLayer);
            var uCharacterRef = Owner.GetDataManaged<UCharacterRef>();
            uCharacterRef.UActor = instance.GetComponent<IUCharacter>();
        }

        private void ClearSkin() {
            var reference = Owner.GetDataManaged<UCharacterRef>();
        }
    }
}