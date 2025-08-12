using UnityEngine;

namespace KillCam.Client {
    public class VisualHUD : Feature {
        private Hud ui;
        
        protected override void OnSetup() {
            var asset = Resources.Load<GameObject>("HUDCanvas");
            var obj = Object.Instantiate(asset);
            obj.TryGetComponent(out ui);
        }

        protected override void OnTickActive() {
            
        }
    }
}