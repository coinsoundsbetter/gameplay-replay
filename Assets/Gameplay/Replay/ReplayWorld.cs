public class ReplayWorld : WorldBase {

    protected override void OnBeforeFeatureInitialize(FeatureBuilder builder) {
        builder.RegisterFeature<WorldEvents>();
        builder.RegisterFeature<ServerGameState>();
        builder.RegisterFeature<ServerRoleRegistry>();
        builder.RegisterFeature<ServerRoleMovement>();
        builder.RegisterFeature<ServerNetworking>();
    }
}