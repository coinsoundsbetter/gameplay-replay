public class ServerWorld : WorldBase {

    protected override void OnBeforeFeatureInitialize(FeatureBuilder builder) {
        builder.RegisterFeature<WorldEvents>();
        builder.RegisterFeature<ServerGameState>();
        builder.RegisterFeature<ServerRoleRegistry>();
        builder.RegisterFeature<ServerRoleMovement>();
        builder.RegisterFeature<ServerNetworking>();
        builder.RegisterFeature<ServerRoleStateSync>();
    }

    protected override void OnAfterInitializeAllFeatures() {
        GetFeature<ServerNetworking>().StartConnect();
    }
}
