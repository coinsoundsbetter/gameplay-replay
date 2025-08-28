using UnityEngine;

namespace KillCam.Client {
    public class VisualHUD : Feature {
        private Hud ui;
        
        protected override void OnCreate() {
            var asset = Resources.Load<GameObject>("HUDCanvas");
            var obj = Object.Instantiate(asset);
            obj.TryGetComponent(out ui);
        }

        protected override void OnTick() {
            
        }
    }
}