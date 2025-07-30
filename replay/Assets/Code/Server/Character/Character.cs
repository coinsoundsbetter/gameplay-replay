namespace KillCam.Server {
    public sealed class Character : GameplayActor {
        public IServerCharacterNet Net { get; set; }
    }
}