namespace KillCam.Client {
    /// <summary>
    /// 负责把客户端的输入转到角色上
    /// </summary>
    public class HeroInput : Feature {
        
        public override bool OnShouldActivate() {
            var data = GetDataRO<HeroIdentifier>();
            if (data.IsControlTarget) {
                return true;
            }

            return false;
        }

        protected override void OnTick() {
            var input = GetWorldData<UserInputState>();
            ref var heroInput = ref GetDataRef<HeroInputState>();
            heroInput.Move = input.Move;
            heroInput.Pitch = input.Pitch;
            heroInput.Yaw = input.Yaw;
            heroInput.IsFirePressed = input.IsFirePressed;
        }
    }
}