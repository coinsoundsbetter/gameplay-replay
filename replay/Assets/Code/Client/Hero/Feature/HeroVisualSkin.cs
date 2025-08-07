using UnityEngine;

namespace KillCam.Client {
    public class HeroVisualSkin : Feature {
        private GameObject instance;
        
        protected override void OnActivate() {
            LoadSkin();
        }

        protected override void OnDeactivate() {
            ClearSkin();
        }

        private void LoadSkin() {
            var asset = Resources.Load("Shared/Hero");
            instance = (GameObject)Object.Instantiate(asset);
            instance.SetLayerRecursively(World.HasFlag(WorldFlag.Replay)
                ? LayerDefine.replayCharacterLayer
                : LayerDefine.characterLayer);
            var uCharacterRef = Owner.GetDataManaged<UnityHeroLink>();
            uCharacterRef.Actor = instance.GetComponent<IUnityHero>();
        }

        private void ClearSkin() {
            var reference = Owner.GetDataManaged<UnityHeroLink>();
            reference.Actor = null;
            Object.Destroy(instance);
        }
    }
}