using UnityEngine;

namespace KillCam.Client {
    public class HeroVisualSkin : SystemBase {
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
            instance.SetLayerRecursively(HasWorldFlag(WorldFlag.Replay)
                ? LayerDefine.replayCharacterLayer
                : LayerDefine.characterLayer);
            var uCharacterRef = GetDataManaged<UnityHeroLink>();
            uCharacterRef.Actor = instance.GetComponent<IUnityHero>();
        }

        private void ClearSkin() {
            var reference = GetDataManaged<UnityHeroLink>();
            reference.Actor = null;
            Object.Destroy(instance);
        }
    }
}