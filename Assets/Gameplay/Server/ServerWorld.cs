public class ServerWorld : WorldBase {

    protected override void OnBeforeFeatureInitialize(FeatureBuilder builder) {
        builder.RegisterFeature<WorldEvents>();
        builder.RegisterFeature<ServerGameMain>();
        builder.RegisterFeature<ServerRoleRegistry>();
        builder.RegisterFeature<ServerNetObjSpawn>();
        builder.RegisterFeature<ServerGameNetRegistry>();
        builder.RegisterFeature<ServerRoleMovement>();
        builder.RegisterFeature<ServerFishNet>();
        builder.RegisterFeature<ServerRoleStateSync>();
    }

    protected override void OnAfterInitializeAllFeatures() {
        GetFeature<ServerGameMain>().StartGame();
    }
}
