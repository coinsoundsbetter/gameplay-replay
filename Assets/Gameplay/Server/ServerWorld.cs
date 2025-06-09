public class ServerWorld : WorldBase {

    protected override void OnBeforeFeatureInitialize(FeatureBuilder builder) {
        builder.RegisterFeature<WorldEvents>();
        builder.RegisterFeature<ServerGameState>();
        builder.RegisterFeature<ServerRoleRegistry>();
        builder.RegisterFeature<ServerRoleNetRegistry>();
        builder.RegisterFeature<ServerGameNetRegistry>();
        builder.RegisterFeature<ServerRoleMovement>();
        builder.RegisterFeature<ServerFishNet>();
        builder.RegisterFeature<ServerRoleStateSync>();
    }

    protected override void OnAfterInitializeAllFeatures() {
        GetFeature<ServerFishNet>().StartConnect();
    }
}
