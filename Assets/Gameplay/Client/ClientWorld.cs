public class ClientWorld : WorldBase {

    protected override void OnBeforeFeatureInitialize(FeatureBuilder builder) {
        builder.RegisterData<GameplayData>();
        builder.RegisterFeature<WorldEvents>();
        builder.RegisterFeature<ClientGameState>();
        builder.RegisterFeature<ClientInputs>();
        builder.RegisterFeature<ClientRoleStateUpdater>();
        builder.RegisterFeature<ClientNetworkSpawn>();
        builder.RegisterFeature<ClientRoleRegistry>();
        builder.RegisterFeature<ClientRoleMovement>();
        builder.RegisterFeature<ClientNetworking>();
        builder.RegisterFeature<Sound>();
        builder.RegisterFeature<UIDisplay>();
    }

    protected override void OnAfterInitializeAllFeatures() {
        GetFeature<ClientNetworking>().StartConnect();
    }
}
